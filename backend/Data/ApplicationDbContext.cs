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
                entity.Property(u => u.Bio).HasColumnName("bio");
                entity.Property(u => u.ProfilePictureUrl).HasColumnName("profile_picture_url");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("posts");
                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.UserId).HasColumnName("user_id");
                entity.Property(u => u.PhotoUrl).HasColumnName("image_url");
                entity.Property(u => u.Caption).HasColumnName("caption");
                entity.Property(u => u.CreatedAt).HasColumnName("created_at");
                entity.Property(u => u.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.ToTable("likes");
                //entity.Property(u => u.Id).HasColumnName("id");
                entity.HasKey(l => new {l.UserId, l.PostId});
                entity.Property(u => u.UserId).HasColumnName("user_id");
                entity.Property(u => u.PostId).HasColumnName("post_id");
                entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");
                entity.Property(u => u.Id).HasColumnName("id");
                entity.Property(u => u.PostId).HasColumnName("post_id");
                entity.Property(u => u.UserId).HasColumnName("user_id");
                entity.Property(u => u.Content).HasColumnName("content");
                entity.Property(u => u.CreatedAt).HasColumnName("created_at");
            });
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
    }
}