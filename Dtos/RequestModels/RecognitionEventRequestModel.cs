namespace SwiftIn.Dtos.RequestModels
{
    // Internal: what the camera worker passes to CheckInService after watching a face across frames.
    public class RecognitionEventRequestModel
    {
        public string CameraId { get; set; }
        public string? TeacherId { get; set; }               // null when nobody matched
        public Guid? BestCandidateTeacherId { get; set; }  // closest teacher when there was no confident match
        public float Confidence { get; set; }
        public int FramesMatched { get; set; }
        public int FaceWidthPx { get; set; }
        public string? SnapshotPath { get; set; }
    }
}
