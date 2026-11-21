namespace SwiftIn.Dtos.ResponseModels
{
    public class EnrollmentResponseModel
    {
        public Guid TeacherId { get; set; }
        public int PhotosAccepted { get; set; }
        public int EmbeddingsCreated { get; set; }   // includes the blurred "degraded" copies
        public List<PhotoRejectionModel> RejectedPhotos { get; set; } = new();
    }
}
