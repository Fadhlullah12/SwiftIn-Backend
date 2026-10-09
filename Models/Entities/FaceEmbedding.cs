using SwiftIn.Models.Enums;

namespace SwiftIn.Models.Entities
{
    public class FaceEmbedding : BaseEntity
    {
        public string TeacherId { get; set; } = default!;
        public Teacher Teacher { get; set; } = default!;
    
        // 512 x float32 (ArcFace) stored as raw bytes = 2048 bytes. Encrypt at rest.
        public byte[] Vector { get; set; } = default!;
        public EmbeddingSource Source { get; set; } = EmbeddingSource.Enrollment;
        public float QualityScore { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
        public static byte[] Pack(float[] v)
        {
            var bytes = new byte[v.Length * sizeof(float)];
            Buffer.BlockCopy(v, 0, bytes, 0, bytes.Length);
            return bytes;
        }
    
        public float[] Unpack()
        {
            var v = new float[Vector.Length / sizeof(float)];
            Buffer.BlockCopy(Vector, 0, v, 0, Vector.Length);
            return v;
        }
    }
}