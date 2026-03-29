using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.Email).HasColumnName("email");
                entity.Property(u => u.Username).HasColumnName("username");
                entity.Property(u => u.PassHash).HasColumnName("password_hash");
                entity.Property(u => u.CreatedAt).HasColumnName("created_at");
                entity.Property(u => u.UpdatedAt).HasColumnName("updated_at");
            });
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
    }
}