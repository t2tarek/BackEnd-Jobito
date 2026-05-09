using CompanyDashboardAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CompanyDashboardAPI.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<JobApplication> JobApplications { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Favorite> Favorites { get; set; } = null!;
    public DbSet<Rating> Ratings { get; set; } = null!;
    public DbSet<Testimonial> Testimonials { get; set; } = null!;
    public DbSet<HelpCategory> HelpCategories { get; set; } = null!;
    public DbSet<HelpArticle> HelpArticles { get; set; } = null!;

    // Keeping these old tables if they are still needed for Daily Stats or Updates, but they might need refactoring
    public DbSet<DailyStat> DailyStats { get; set; } = null!;
    public DbSet<JobUpdate> JobUpdates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Define many-to-many relationship for Category and Job (JobsMany)
        modelBuilder.Entity<Job>()
            .HasMany(j => j.Categories)
            .WithMany(c => c.JobsMany)
            .UsingEntity(j => j.ToTable("JobCategories"));

        // Unique constraint for Favorites
        modelBuilder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.JobId })
            .IsUnique();

        // Unique constraint for JobApplications
        modelBuilder.Entity<JobApplication>()
            .HasIndex(a => new { a.UserId, a.JobId })
            .IsUnique();

        // One-to-Many relationships configuration (most are automatically handled by EF Core conventions)
        modelBuilder.Entity<Job>()
            .HasOne(j => j.Company)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Job>()
            .HasOne(j => j.User)
            .WithMany(u => u.Jobs)
            .HasForeignKey(j => j.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Job>()
            .HasOne(j => j.Category)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Precision for Salary
        modelBuilder.Entity<Job>()
            .Property(j => j.Salary)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Job>()
            .Property(j => j.SalaryMin)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Job>()
            .Property(j => j.SalaryMax)
            .HasPrecision(18, 2);
    }
}
