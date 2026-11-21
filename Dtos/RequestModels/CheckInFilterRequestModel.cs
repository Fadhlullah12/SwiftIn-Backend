using SwiftIn.Models.Enums;

namespace SwiftIn.Dtos.RequestModels
{
    // Query-string filters, e.g. /api/checkins?status=NeedsReview&page=2
    public class CheckInFilterRequestModel
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Guid? TeacherId { get; set; }
        public CheckInStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
