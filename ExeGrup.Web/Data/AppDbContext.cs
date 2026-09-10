using ExeGrup.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ExeGrup.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectImage> ProjectImages => Set<ProjectImage>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<AdminUser>().HasIndex(x => x.Username).IsUnique();
        mb.Entity<Project>().HasIndex(x => x.Slug).IsUnique();
        mb.Entity<Service>().HasIndex(x => x.Slug).IsUnique();
        mb.Entity<SiteSetting>().HasIndex(x => x.Key).IsUnique();

        mb.Entity<ProjectImage>()
            .HasOne(x => x.Project)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}