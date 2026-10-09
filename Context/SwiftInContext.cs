using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SwiftIn.Models.Entities;

namespace SwiftIn.Context;

public class SwiftInContext : IdentityDbContext<ApplicationUser>
{
    public SwiftInContext(DbContextOptions<SwiftInContext> options) : base(options) { }

    public DbSet<Camera> Cameras { get; set; }
    public DbSet<CheckIn> CheckIns { get; set; }
    public DbSet<FaceEmbedding> FaceEmbeddings { get; set; }   // renamed from FaceEmbebbing
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<UnknownDetection> UnknownDetections { get; set; } // renamed from UknownDetection

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // required for Identity's tables

        // Teacher
        modelBuilder.Entity<Teacher>(e =>
        {
            e.Property(x => x.StaffNumber).IsRequired().HasMaxLength(50);
            e.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.StaffNumber).IsUnique();
            e.HasMany(x => x.Embeddings).WithOne(x => x.Teacher)
            .HasForeignKey(x => x.TeacherId);
        });

        //  FaceEmbedding 
        modelBuilder.Entity<FaceEmbedding>(e =>
        {
            e.Property(x => x.Vector).IsRequired();
            e.Property(x => x.Source).HasConversion<string>().HasMaxLength(20);

            // Deleting a teacher also deletes their face data
            e.HasOne(x => x.Teacher)
             .WithMany(t => t.Embeddings)
             .HasForeignKey(x => x.TeacherId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        //  Camera 
        modelBuilder.Entity<Camera>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Direction).HasConversion<string>().HasMaxLength(10);
        });

        //  CheckIn 
        modelBuilder.Entity<CheckIn>(e =>
        {
            e.Property(x => x.Direction).HasConversion<string>().HasMaxLength(10);
            e.Property(x => x.Method).HasConversion<string>().HasMaxLength(10);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

            // Can't delete a teacher who has check-ins (keeps attendance history)
            e.HasOne(x => x.Teacher)
             .WithMany(t => t.CheckIns)
             .HasForeignKey(x => x.TeacherId)
             .OnDelete(DeleteBehavior.Restrict);

            // Deleting a camera keeps its check-ins
            e.HasOne(x => x.Camera)
             .WithMany()
             .HasForeignKey(x => x.CameraId)
             .OnDelete(DeleteBehavior.SetNull);

            e.HasIndex(x => new { x.TeacherId, x.Timestamp });
            e.HasIndex(x => x.Status);
        });

        //  UnknownDetection 
        modelBuilder.Entity<UnknownDetection>(e =>
        {
            e.HasIndex(x => x.Timestamp);
        });
    }
}