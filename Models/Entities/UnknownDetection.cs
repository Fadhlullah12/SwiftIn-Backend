namespace SwiftIn.Models.Entities
{
    public class UnknownDetection : BaseEntity
    {
        public string? CameraId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
        public string? BestCandidateWorkerId { get; set; }  // closest worker, if any
        public float? BestScore { get; set; }
        public int FaceWidthPx { get; set; }              // how big the face was in the frame
        public string? SnapshotPath { get; set; }   
    }
}