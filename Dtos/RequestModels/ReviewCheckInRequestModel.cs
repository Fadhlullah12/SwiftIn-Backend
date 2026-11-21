namespace SwiftIn.Dtos.RequestModels
{
    public class ReviewCheckInRequestModel
    {
        // true = Confirmed, false = Rejected
        public bool Approve { get; set; }
        public string? Note { get; set; }
    }
}
