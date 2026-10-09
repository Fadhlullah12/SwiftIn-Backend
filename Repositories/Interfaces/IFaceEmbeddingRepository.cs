using SwiftIn.Models.Entities;

namespace SwiftIn.Repositories;

public interface IFaceEmbeddingRepository : IBaseRepository<FaceEmbedding>
{
    Task<List<FaceEmbedding>> GetByTeacherAsync(string teacherId);
    Task AddRangeAsync(IEnumerable<FaceEmbedding> embeddings);
    Task DeleteByTeacherAsync(string teacherId);
}