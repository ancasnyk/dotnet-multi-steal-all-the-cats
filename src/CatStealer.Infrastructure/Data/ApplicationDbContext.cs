using CatStealer.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatStealer.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        
        public DbSet<CatEntity> Cats { get; set; }
        public DbSet<TagEntity> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.CatId).IsRequired();
                entity.Property(e => e.Width).IsRequired();
                entity.Property(e => e.Height).IsRequired();
                entity.Property(e => e.Image).IsRequired();
                entity.Property(e => e.Created).IsRequired();

                entity.HasIndex(e => e.CatId).IsUnique();
            });

            modelBuilder.Entity<TagEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).UseIdentityColumn();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Created).IsRequired();

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<CatEntity>()
                .HasMany(c => c.Tags)
                .WithMany(t => t.Cats)
                .UsingEntity(j => j.ToTable("CatTags"));
        }
    }
}
