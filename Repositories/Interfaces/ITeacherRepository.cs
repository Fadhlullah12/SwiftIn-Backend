using SwiftIn.Models.Entities;

namespace SwiftIn.Repositories;

public interface ITeacherRepository : IBaseRepository<Teacher>
{
    Task<Teacher?> GetByStaffNumberAsync(string staffNumber);
    Task<bool> StaffNumberExistsAsync(string staffNumber);
    Task<List<Teacher>> GetActiveWithEmbeddingsAsync();
}