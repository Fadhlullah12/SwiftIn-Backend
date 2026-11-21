using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SwiftIn.Dtos.RequestModels;
using SwiftIn.Services;
using SwiftIn.Services.Interface;
using SwiftIn.Settings;

namespace SwiftIn.Controllers
{
    [Authorize]   // Admin and Reviewer can both see and review; manual entry is Admin only
    public class CheckInsController : ApiControllerBase
    {
        private readonly ICheckInService _checkIns;
        private readonly CheckInSettings _settings;

        public CheckInsController(ICheckInService checkIns, IOptions<CheckInSettings> settings)
        {
            _checkIns = checkIns;
            _settings = settings.Value;
        }

        // GET api/checkins?status=NeedsReview&teacherId=...&from=...&to=...&page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetCheckIns([FromQuery] CheckInFilterRequestModel filter)
        {
            return FromResponse(await _checkIns.GetCheckInsAsync(filter));
        }

        // GET api/checkins/attendance?date=2026-11-06   (date is optional: defaults to today)
        [HttpGet("attendance")]
        public async Task<IActionResult> GetDailyAttendance([FromQuery] DateOnly? date)
        {
            var day = date ?? TodayInSchoolTime();
            return FromResponse(await _checkIns.GetDailyAttendanceAsync(day));
        }

        // POST api/checkins/manual
        [HttpPost("manual")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> CreateManual([FromBody] ManualCheckInRequestModel request)
        {
            return FromResponse(await _checkIns.CreateManualAsync(request, CurrentUserName));
        }

        // POST api/checkins/{id}/review
        [HttpPost("{id:guid}/review")]
        public async Task<IActionResult> Review(Guid id, [FromBody] ReviewCheckInRequestModel request)
        {
            return FromResponse(await _checkIns.ReviewAsync(id, request, CurrentUserName));
        }

        private DateOnly TodayInSchoolTime()
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(_settings.TimeZoneId);
            return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz));
        }
    }
}