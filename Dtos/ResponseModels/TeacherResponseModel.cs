using SwiftIn.Models.Entities;

namespace SwiftIn.Dtos.ResponseModels
{
    // What the outside world may see about a teacher. No Embeddings or CheckIns lists on purpose.
    public class TeacherResponseModel
    {
        public string Id { get; set; }
        public string StaffNumber { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime? ConsentGivenAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public static TeacherResponseModel FromEntity(Teacher t) => new()
        {
            Id = t.Id,
            StaffNumber = t.StaffNumber,
            FullName = t.FullName,
            IsActive = t.IsActive,
            ConsentGivenAt = t.ConsentGivenAt,
            CreatedAt = t.CreatedAt
        };
    }
}
