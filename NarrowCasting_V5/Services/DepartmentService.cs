using Microsoft.EntityFrameworkCore;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuditService _audit;

        public DepartmentService(ApplicationDbContext db, IAuditService audit)
        {
            _db = db;
            _audit = audit;
        }
        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _db.Departments.Include(d => d.Screens).OrderBy(d => d.Name).ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _db.Departments.Include(d => d.Screens).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task CreateAsync(Department department, string userId)
        {
            _db.Departments.Add(department);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Department", department.Id, "Create", userId);
        }

        public async Task UpdateAsync(Department department, string userId)
        {
            _db.Departments.Update(department);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Department", department.Id, "Update", userId);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept is null) return;
            _db.Departments.Remove(dept);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Department", id, "Delete", userId);
        }
    }
}
