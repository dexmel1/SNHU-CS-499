using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DungeonExplorer.Data
{
    public class GameDbContext : DbContext
    {
        public DbSet<PlayerEntity> Players => Set<PlayerEntity>();
        public DbSet<ScoreEntity> Scores => Set<ScoreEntity>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Store database in local app folder next to the exe
                var basePath = Directory.GetCurrentDirectory();
                var dbPath = Path.Combine(basePath, "dungeon_scores.db");

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity((System.Action<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ScoreEntity>>)(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasOne(s => s.Player)
                      .WithMany(p => p.Scores)
                      .HasForeignKey(s => s.PlayerId);
            }));
        }
    }
}
