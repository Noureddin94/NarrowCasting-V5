using NarrowCasting_V5.Enums;
using System.ComponentModel.DataAnnotations;

namespace NarrowCasting_V5.Models
{
    public class MediaFile
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public MediaType MediaType { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // FK
        [Required]
        public string? UploadedById { get; set; }

        // Navigation properties
        public ApplicationUser? UploadedBy { get; set; }
        public ICollection<PlaylistItem> PlaylistItems { get; set; } = new List<PlaylistItem>();

        [MaxLength(500)]
        public string? Caption { get; set; }
    }
}
