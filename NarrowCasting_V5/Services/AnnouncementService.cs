using Microsoft.EntityFrameworkCore;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuditService _audit;

        public AnnouncementService(ApplicationDbContext db, IAuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        public async Task<IEnumerable<Announcement>> GetAllAsync()
        {
            return await _db.Announcements.Include(a => a.Department)
                                          .Include(a => a.CreatedBy)  
                                          .OrderByDescending(a => a.PublishedAt)
                                          .ToListAsync();
        }

        public async Task<IEnumerable<Announcement>> GetActiveAsync()
        {
            return await _db.Announcements.Include(a => a.Department)
                                          .Where(a => a.ExpiresAt == null || a.ExpiresAt > DateTime.Now)
                                          .ToListAsync();
        }

        public async Task<IEnumerable<Announcement>> GetActiveForDepartmentAsync(int departmentId)
        {
            return await _db.Announcements.Include(a => a.Department)
                                          .Where(a => a.DepartmentId == departmentId 
                                                && (a.ExpiresAt == null || a.ExpiresAt > DateTime.Now))
                                          .ToListAsync();
        }

        public async Task<Announcement?> GetByIdAsync(int id)
        {
            return await _db.Announcements.Include(a => a.Department)
                                          .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task CreateAsync(Announcement announcement, string userId)
        {
            _db.Announcements.Add(announcement);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Announcement", announcement.Id, "Create", userId);
        }

        public async Task UpdateAsync(Announcement announcement, string userId)
        {
            _db.Announcements.Update(announcement);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Announcement", announcement.Id, "Update", userId);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var announcement = await _db.Announcements.FindAsync(id);
            if (announcement is null) return;
            _db.Announcements.Remove(announcement);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Announcement", id, "Delete", userId);
        }
    }
}
