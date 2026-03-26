using System;
using System.ComponentModel.DataAnnotations;

namespace ArtPort.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}