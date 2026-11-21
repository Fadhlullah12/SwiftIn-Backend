namespace SwiftIn.Settings
{
    // Bound from the "CheckIn" section of appsettings.json. Tune these with real gate footage.
    public class CheckInSettings
    {
        public float ConfirmThreshold { get; set; } = 0.60f;   // at/above this (with enough frames) -> Confirmed
        public float ReviewThreshold { get; set; } = 0.40f;    // between Review and Confirm -> NeedsReview; below -> unknown
        public int MinFramesToConfirm { get; set; } = 3;       // frames that must agree for auto-confirm
        public int CooldownMinutes { get; set; } = 5;          // ignore repeat check-ins within this window
        public string TimeZoneId { get; set; } = "Africa/Lagos"; // decides where "today" starts and ends
    }
}