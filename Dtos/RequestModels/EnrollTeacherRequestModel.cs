using Microsoft.AspNetCore.Http;

namespace SwiftIn.Dtos.RequestModels
{
    public class EnrollTeacherRequestModel
    {
        public Guid TeacherId { get; set; }
        public List<IFormFile> Photos { get; set; } = new();
    }
}
