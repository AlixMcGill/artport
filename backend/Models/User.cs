using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Username {get; set;} = null!;

        [Required, MaxLength(255)]
        public string PassHash { get; set; } = null!;

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(500)]
        public string? ProfilePictureUrl { get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}