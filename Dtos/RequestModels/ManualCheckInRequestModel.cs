using SwiftIn.Models.Enums;

namespace SwiftIn.Dtos.RequestModels
{
    public class ManualCheckInRequestModel
    {
        public Guid TeacherId { get; set; }
        public CheckInDirection Direction { get; set; }
        public string? Note { get; set; }
    }
}
