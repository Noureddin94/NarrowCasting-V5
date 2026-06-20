using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NarrowCasting_V5.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        public int? DepartmentId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Department? Department { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; } = [];
        public ICollection<MediaFile> UploadedFiles { get; set; } = [];
        public ICollection<Playlist> CreatedPlaylists { get; set; } = [];
        public ICollection<Announcement> CreatedAnnouncements { get; set; } = [];
    }
}
