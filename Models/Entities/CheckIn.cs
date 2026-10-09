using SwiftIn.Models.Enums;

namespace SwiftIn.Models.Entities
{
    public class CheckIn : BaseEntity
    {
 
            public string? TeacherId { get; set; }
            public Teacher Teacher { get; set; } = default!;
        
            public string? CameraId { get; set; }          // null for manual check-ins
            public Camera? Camera { get; set; }
        
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
            public CheckInDirection Direction { get; set; } = CheckInDirection.In;
            public CheckInMethod Method { get; set; } = CheckInMethod.Face;
            public CheckInStatus Status { get; set; } = CheckInStatus.Confirmed;
        
            public float? Confidence { get; set; }       // how similar the match was
            public int FramesMatched { get; set; }       // how many video frames agreed
            public string? SnapshotPath { get; set; }    // saved photo as proof
        
            // Filled in when an admin reviews it
            public string? ReviewedByUserId { get; set; }
            public ApplicationUser? ReviewedByUser { get; set; }
            public DateTime? ReviewedAt { get; set; }
            public string? Note { get; set; }
    }
}