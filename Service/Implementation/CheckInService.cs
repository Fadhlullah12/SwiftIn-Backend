using Microsoft.Extensions.Options;
using SwiftIn.Dtos.RequestModels;
using SwiftIn.Dtos.ResponseModels;
using SwiftIn.Models.Entities;
using SwiftIn.Models.Enums;
using SwiftIn.Repositories;
using SwiftIn.Services.Interface;
using SwiftIn.Settings;
// using SwiftIn.Settings;

namespace SwiftIn.Services.Implementation
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkIns;
        private readonly ITeacherRepository _teachers;
        private readonly IBaseRepository<Camera> _cameras;
        private readonly IBaseRepository<UnknownDetection> _unknowns;
        private readonly CheckInSettings _settings;

        public CheckInService(
            ICheckInRepository checkIns,
            ITeacherRepository teachers,
            IBaseRepository<Camera> cameras,
            IBaseRepository<UnknownDetection> unknowns,
            IOptions<CheckInSettings> settings)
        {
            _checkIns = checkIns;
            _teachers = teachers;
            _cameras = cameras;
            _unknowns = unknowns;
            _settings = settings.Value;
        }

        // ---------- Camera worker -> check-in ----------
        public async Task<BaseResponse<CheckInResponseModel?>> RecordRecognitionAsync(RecognitionEventRequestModel request)
        {
            var camera = await _cameras.GetByIdAsync(request.CameraId);
            if (camera is null)
                return BaseResponse<CheckInResponseModel?>.Fail("Camera not found.");

            // Only trust the match if it's an active teacher and the score clears the review threshold
            Teacher? teacher = null;
            if (request.TeacherId != null && request.Confidence >= _settings.ReviewThreshold)
            {
                teacher = await _teachers.GetByIdAsync(request.TeacherId);
                if (teacher is not null && !teacher.IsActive)
                    teacher = null;
            }

            // Not recognised: keep a record so thresholds can be tuned later
            if (teacher is null)
            {
                await _unknowns.AddAsync(new UnknownDetection
                {
                    CameraId = camera.Id,
                    Timestamp = DateTime.UtcNow,
                    BestCandidateTeacherId = request.BestCandidateTeacherId.ToString() ?? request.TeacherId.ToString(),
                    BestScore = request.Confidence,
                    FaceWidthPx = request.FaceWidthPx,
                    SnapshotPath = request.SnapshotPath
                });

                return BaseResponse<CheckInResponseModel?>.Ok(null, "Face not recognised; saved for tuning.");
            }

            // Cooldown: one person standing at the gate must not create 50 records
            var latest = await _checkIns.GetLatestForTeacherAsync(teacher.Id);
            if (latest is not null
                && latest.Direction == camera.Direction
                && DateTime.UtcNow - latest.Timestamp < TimeSpan.FromMinutes(_settings.CooldownMinutes))
            {
                return BaseResponse<CheckInResponseModel?>.Ok(null, "Ignored: already checked in recently.");
            }

            // Confident and seen in enough frames -> Confirmed. Otherwise a human should look.
            var status = request.Confidence >= _settings.ConfirmThreshold
                         && request.FramesMatched >= _settings.MinFramesToConfirm
                ? CheckInStatus.Confirmed
                : CheckInStatus.NeedsReview;

            var checkIn = new CheckIn
            {
                TeacherId = teacher.Id,
                CameraId = camera.Id,
                Timestamp = DateTime.UtcNow,
                Direction = camera.Direction,
                Method = CheckInMethod.Face,
                Status = status,
                Confidence = request.Confidence,
                FramesMatched = request.FramesMatched,
                SnapshotPath = request.SnapshotPath
            };

            await _checkIns.AddAsync(checkIn);

            // Attach for mapping only (after saving)
            checkIn.Teacher = teacher;
            checkIn.Camera = camera;

            return BaseResponse<CheckInResponseModel?>.Ok(
                CheckInResponseModel.FromEntity(checkIn),
                status == CheckInStatus.Confirmed ? "Check-in confirmed." : "Check-in saved for review.");
        }

        // ---------- Admin fallback ----------
        public async Task<BaseResponse<CheckInResponseModel>> CreateManualAsync(ManualCheckInRequestModel request, ApplicationUser performedBy)
        {
            var teacher = await _teachers.GetByIdAsync(request.TeacherId.ToString());
            if (teacher is null)
                return BaseResponse<CheckInResponseModel>.Fail("Teacher not found.");
            if (!teacher.IsActive)
                return BaseResponse<CheckInResponseModel>.Fail("Teacher is not active.");

            var checkIn = new CheckIn
            {
                TeacherId = teacher.Id,
                CameraId = null,
                Timestamp = DateTime.UtcNow,
                Direction = request.Direction,
                Method = CheckInMethod.Manual,
                Status = CheckInStatus.Confirmed,   // an admin entered it, so it's trusted
                ReviewedByUser = performedBy,
                ReviewedAt = DateTime.UtcNow,
                Note = request.Note?.Trim()
            };

            await _checkIns.AddAsync(checkIn);
            checkIn.Teacher = teacher;

            return BaseResponse<CheckInResponseModel>.Ok(
                CheckInResponseModel.FromEntity(checkIn), "Manual check-in recorded.");
        }

        // ---------- Review queue ----------
        public async Task<BaseResponse<CheckInResponseModel>> ReviewAsync(string checkInId, ReviewCheckInRequestModel request, ApplicationUser reviewedBy)
        {
            var checkIn = await _checkIns.GetByIdAsync(checkInId);
            if (checkIn is null)
                return BaseResponse<CheckInResponseModel>.Fail("Check-in not found.");

            if (checkIn.Status != CheckInStatus.NeedsReview)
                return BaseResponse<CheckInResponseModel>.Fail("Only check-ins that need review can be reviewed.");

            checkIn.Status = request.Approve ? CheckInStatus.Confirmed : CheckInStatus.Rejected;
            checkIn.ReviewedByUser = reviewedBy;
            checkIn.ReviewedAt = DateTime.UtcNow;
            checkIn.Note = request.Note?.Trim();

            await _checkIns.UpdateAsync(checkIn);

            checkIn.Teacher = await _teachers.GetByIdAsync(checkIn.TeacherId) ?? checkIn.Teacher;

            return BaseResponse<CheckInResponseModel>.Ok(
                CheckInResponseModel.FromEntity(checkIn),
                request.Approve ? "Check-in approved." : "Check-in rejected.");
        }

        // ---------- Listing ----------
        public async Task<BaseResponse<PagedResponse<CheckInResponseModel>>> GetCheckInsAsync(CheckInFilterRequestModel filter)
        {
            var page = Math.Max(filter.Page, 1);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);

            var (items, total) = await _checkIns.SearchAsync(
                filter.From, filter.To, filter.TeacherId, filter.Status, page, pageSize);

            var result = new PagedResponse<CheckInResponseModel>
            {
                Items = items.Select(CheckInResponseModel.FromEntity).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };

            return BaseResponse<PagedResponse<CheckInResponseModel>>.Ok(result, "Check-ins retrieved successfully.");
        }

        // ---------- Dashboard summary ----------
        public async Task<BaseResponse<DailyAttendanceResponseModel>> GetDailyAttendanceAsync(DateOnly date)
        {
            // "Today" means the school's local day, not the UTC day
            var tz = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZoneId);
            var localStart = date.ToDateTime(TimeOnly.MinValue);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, tz);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(localStart.AddDays(1), tz);

            var activeTeachers = (await _teachers.GetAllAsync()).Where(t => t.IsActive).ToList();
            var dayCheckIns = await _checkIns.GetByDateRangeAsync(startUtc, endUtc);
            var byTeacher = dayCheckIns.GroupBy(c => c.TeacherId).ToDictionary(g => g.Key, g => g.ToList());

            var rows = new List<AttendanceRowModel>();
            foreach (var teacher in activeTeachers.OrderBy(t => t.FullName))
            {
                byTeacher.TryGetValue(teacher.Id, out var list);
                list ??= new List<CheckIn>();

                var valid = list.Where(c => c.Status != CheckInStatus.Rejected).ToList();
                var ins = valid.Where(c => c.Direction == CheckInDirection.In).OrderBy(c => c.Timestamp).ToList();
                var outs = valid.Where(c => c.Direction == CheckInDirection.Out).OrderByDescending(c => c.Timestamp).ToList();

                rows.Add(new AttendanceRowModel
                {
                    TeacherId = teacher.Id,
                    TeacherName = teacher.FullName,
                    IsPresent = ins.Count > 0,
                    FirstIn = ins.Count > 0 ? ins[0].Timestamp : null,
                    LastOut = outs.Count > 0 ? outs[0].Timestamp : null,
                    HasUnreviewedCheckIns = list.Any(c => c.Status == CheckInStatus.NeedsReview)
                });
            }

            var summary = new DailyAttendanceResponseModel
            {
                Date = date,
                TotalActiveTeachers = activeTeachers.Count,
                Present = rows.Count(r => r.IsPresent),
                Absent = rows.Count(r => !r.IsPresent),
                NeedingReview = dayCheckIns.Count(c => c.Status == CheckInStatus.NeedsReview),
                Rows = rows
            };

            return BaseResponse<DailyAttendanceResponseModel>.Ok(summary, "Attendance retrieved successfully.");
        }
        
    }
}