using Microsoft.EntityFrameworkCore;
using SwiftIn.Context;
using SwiftIn.Models.Entities;

namespace SwiftIn.Repositories;

public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
{
    public TeacherRepository(SwiftInContext db) : base(db) { }

    public async Task<Teacher?> GetByStaffNumberAsync(string staffNumber) =>
        await _set.FirstOrDefaultAsync(t => t.StaffNumber == staffNumber);

    public async Task<bool> StaffNumberExistsAsync(string staffNumber) =>
        await _set.AnyAsync(t => t.StaffNumber == staffNumber);

    public async Task<List<Teacher>> GetActiveWithEmbeddingsAsync() =>
        await _set.Include(t => t.Embeddings)
                  .Where(t => t.IsActive)
                  .AsNoTracking()
                  .ToListAsync();
}