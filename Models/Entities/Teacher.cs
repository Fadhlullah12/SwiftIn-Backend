namespace SwiftIn.Models.Entities
{
    public class Teacher : BaseEntity
    {
        public string StaffNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Department { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
    
        // Biometric consent (required before any enrollment)
        public DateTime? ConsentGivenAt { get; set; }
        public string? ConsentDocumentRef { get; set; }
    
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeactivatedAt { get; set; }
    
        public List<FaceEmbedding> Embeddings { get; set; } = new();
        public List<CheckIn> CheckIns { get; set; } = new();
    }
}