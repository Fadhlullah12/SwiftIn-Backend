namespace SwiftIn.Models.Enums
{
    public enum CheckInStatus
    {
        Confirmed,    // system was confident
        NeedsReview,  // low confidence: a human should look at the snapshot
        Rejected      // reviewed and found to be wrong
    }
}