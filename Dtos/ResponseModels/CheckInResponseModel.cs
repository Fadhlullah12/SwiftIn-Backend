using SwiftIn.Models.Entities;
using SwiftIn.Models.Enums;

namespace SwiftIn.Dtos.ResponseModels
{
    public class CheckInResponseModel
    {
        public string Id { get; set; }
        public string TeacherId { get; set; }
        public string TeacherName { get; set; } = default!;
        public string? CameraName { get; set; }
        public DateTime Timestamp { get; set; }
        public CheckInDirection Direction { get; set; }
        public CheckInMethod Method { get; set; }
        public CheckInStatus Status { get; set; }
        public float? Confidence { get; set; }
        public int FramesMatched { get; set; }
        public string? SnapshotUrl { get; set; }
        public string? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Note { get; set; }

        // Needs c.Teacher loaded (Include). Camera is optional: manual check-ins have none.
        public static CheckInResponseModel FromEntity(CheckIn c) => new()
        {
            Id = c.Id,
            TeacherId = c.TeacherId,
            TeacherName = c.Teacher?.FullName ?? string.Empty,
            CameraName = c.Camera?.Name,
            Timestamp = c.Timestamp,
            Direction = c.Direction,
            Method = c.Method,
            Status = c.Status,
            Confidence = c.Confidence,
            FramesMatched = c.FramesMatched,
            // Never expose the server's disk path; expose a URL (assumes snapshots are served at /snapshots)
            SnapshotUrl = c.SnapshotPath is null ? null : $"/snapshots/{Path.GetFileName(c.SnapshotPath)}",
            ReviewedBy = c.ReviewedByUser?.ToString(),
            ReviewedAt = c.ReviewedAt,
            Note = c.Note
        };
    }
}
