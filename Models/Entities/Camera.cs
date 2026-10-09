using SwiftIn.Models.Enums;

namespace SwiftIn.Models.Entities
{
    public class Camera : BaseEntity
    {
            public string Name { get; set; } = default!;
            public string? Location { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime? LastHeartbeatAt { get; set; }
            public CheckInDirection Direction { get; set; } = CheckInDirection.In;
    }
}