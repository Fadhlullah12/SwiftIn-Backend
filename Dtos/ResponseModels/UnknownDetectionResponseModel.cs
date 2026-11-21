namespace SwiftIn.Dtos.ResponseModels
{
    public class UnknownDetectionResponseModel
    {
        public Guid Id { get; set; }
        public string? CameraName { get; set; }
        public DateTime Timestamp { get; set; }
        public string? BestCandidateName { get; set; }
        public float? BestScore { get; set; }
        public int FaceWidthPx { get; set; }
        public string? SnapshotUrl { get; set; }
    }
}
