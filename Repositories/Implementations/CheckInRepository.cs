using Microsoft.EntityFrameworkCore;
using SwiftIn.Context;
using SwiftIn.Models.Entities;
using SwiftIn.Models.Enums;

namespace SwiftIn.Repositories;

public class CheckInRepository : BaseRepository<CheckIn>, ICheckInRepository
{
    public CheckInRepository(SwiftInContext db) : base(db) { }

    public async Task<List<CheckIn>> GetByTeacherAsync(string teacherId, DateTime fromUtc, DateTime toUtc) =>
        await _set.Where(c => c.TeacherId == teacherId && c.Timestamp >= fromUtc && c.Timestamp < toUtc)
                  .OrderByDescending(c => c.Timestamp)
                  .ToListAsync();

    public async Task<List<CheckIn>> GetByDateRangeAsync(DateTime fromUtc, DateTime toUtc) =>
        await _set.Include(c => c.Teacher)
                  .Where(c => c.Timestamp >= fromUtc && c.Timestamp < toUtc)
                  .OrderByDescending(c => c.Timestamp)
                  .ToListAsync();

    public async Task<List<CheckIn>> GetNeedingReviewAsync() =>
        await _set.Include(c => c.Teacher)
                  .Where(c => c.Status == CheckInStatus.NeedsReview)
                  .OrderByDescending(c => c.Timestamp)
                  .ToListAsync();

    public async Task<CheckIn?> GetLatestForTeacherAsync(string teacherId) =>
        await _set.Where(c => c.TeacherId == teacherId)
                  .OrderByDescending(c => c.Timestamp)
                  .FirstOrDefaultAsync();
}