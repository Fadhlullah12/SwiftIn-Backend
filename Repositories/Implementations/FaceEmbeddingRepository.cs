using Microsoft.EntityFrameworkCore;
using SwiftIn.Context;
using SwiftIn.Models.Entities;

namespace SwiftIn.Repositories;

public class FaceEmbeddingRepository : BaseRepository<FaceEmbedding>, IFaceEmbeddingRepository
{
    public FaceEmbeddingRepository(SwiftInContext db) : base(db) { }

    public async Task<List<FaceEmbedding>> GetByTeacherAsync(string teacherId) =>
        await _set.Where(e => e.TeacherId == teacherId).ToListAsync();

    public async Task AddRangeAsync(IEnumerable<FaceEmbedding> embeddings)
    {
        await _set.AddRangeAsync(embeddings);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteByTeacherAsync(string teacherId)
    {
        await _set.Where(e => e.TeacherId == teacherId).ExecuteDeleteAsync();
    }
}