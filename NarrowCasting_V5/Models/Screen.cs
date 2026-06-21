using System.ComponentModel.DataAnnotations;

namespace NarrowCasting_V5.Models
{
    public class Screen
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsStaffScreen { get; set; } = false;

        // FK
        [Required]
        public int DepartmentId { get; set; }

        // Navigation properties
        public Department Department { get; set; } = null!;
        public ICollection<Playlist> Playlists { get; set; } = [];
        public ICollection<Schedule> Schedules { get; set; } = [];
    }
}
