using SwiftIn.Models.Entities;

namespace SwiftIn.Repositories;

public interface ICheckInRepository : IBaseRepository<CheckIn>
{
    // Check-ins for one teacher in a date range
    Task<List<CheckIn>> GetByTeacherAsync(string teacherId, DateTime fromUtc, DateTime toUtc);

    // Everything in a date range (daily attendance report)
    Task<List<CheckIn>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc);

    // Doubtful check-ins waiting for an admin to review
    Task<List<CheckIn>> GetNeedingReviewAsync();

    // Most recent check-in, used for the "don't record twice within X minutes" rule
    Task<CheckIn?> GetLatestForTeacherAsync(string teacherId);
}