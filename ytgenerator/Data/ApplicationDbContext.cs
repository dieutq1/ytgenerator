using Microsoft.EntityFrameworkCore;
using ytgenerator.Data.Entities;

namespace ytgenerator.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AccessToken> AccessTokens { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            modelBuilder.Entity<AccessToken>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(e => !e.IsDeleted);
                entity.HasOne(t => t.User)
                      .WithMany(u => u.AccessTokens)
                      .HasForeignKey(t => t.UserId);
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);
                entity.HasQueryFilter(e => !e.IsDeleted);
                entity.HasOne(t => t.User)
                      .WithOne(u => u.Setting)
                      .HasForeignKey<Setting>(t => t.UserId);
            });
        }
    }
}
