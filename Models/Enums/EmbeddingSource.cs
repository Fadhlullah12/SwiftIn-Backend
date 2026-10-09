namespace SwiftIn.Models.Enums
{
    public enum EmbeddingSource
    {
         Enrollment,  // from a real photo taken at enrollment
         Degraded     // from a deliberately blurred/shrunk copy (helps matching from far away)
    }
}