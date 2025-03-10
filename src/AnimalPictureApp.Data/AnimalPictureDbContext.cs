using Microsoft.EntityFrameworkCore;
using AnimalPictureApp.Core.Models;

namespace AnimalPictureApp.Data
{
    public class AnimalPictureDbContext : DbContext
    {
        public AnimalPictureDbContext(DbContextOptions<AnimalPictureDbContext> options)
            : base(options)
        {
        }

        public DbSet<AnimalPicture> AnimalPictures { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnimalPicture>()
                .HasIndex(p => p.AnimalType);
            
            modelBuilder.Entity<AnimalPicture>()
                .HasIndex(p => p.StoredAt);
        }
    }
}
