using System.ComponentModel.DataAnnotations;

namespace NarrowCasting_V5.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        [Required]
        public int ScreenId { get; set; }
        public string CreatedById { get; set; } = string.Empty;

        // Navigation
        public Screen Screen { get; set; } = null!;
        public ApplicationUser CreatedBy { get; set; } = null!;
        public ICollection<PlaylistItem> Items { get; set; } = [];
        public ICollection<Schedule> Schedules { get; set; } = [];
    }
}
