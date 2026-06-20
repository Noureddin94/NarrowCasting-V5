using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NarrowCasting_V5.Pages.Admin.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db) => _db = db;

        public int DepartmentCount { get; set; }
        public int ScreenCount { get; set; }
        public int PlaylistCount { get; set; }
        public int AnnouncementCount { get; set; }
        public List<Screen> RecentScreens { get; set; } = new();
        public List<Models.AuditLog> RecentLogs { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Admins see everything; Employees see their own department
            if (User.IsInRole("Admin"))
            {
                DepartmentCount = await _db.Departments.CountAsync();
                ScreenCount = await _db.Screens.CountAsync();
                PlaylistCount = await _db.Playlists.CountAsync();
                AnnouncementCount = await _db.Announcements.CountAsync();
                RecentScreens = await _db.Screens.Include(s => s.Department).Take(5).ToListAsync();
            }
            else
            {
                // Employee: get their department id from Identity claims
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

                int? deptId = null;
                if (user != null)
                {
                    // Try to get DepartmentId via reflection if it exists
                    var prop = user.GetType().GetProperty("DepartmentId");
                    if (prop != null)
                    {
                        deptId = prop.GetValue(user) as int?;
                    }
                }

                ScreenCount = await _db.Screens.CountAsync(s => s.DepartmentId == deptId);
                PlaylistCount = await _db.Playlists.CountAsync(p => p.Screen.DepartmentId == deptId);
                AnnouncementCount = await _db.Announcements.CountAsync(a => a.DepartmentId == deptId);
                RecentScreens = await _db.Screens.Include(s => s.Department)
                                                      .Where(s => s.DepartmentId == deptId)
                                                      .Take(5).ToListAsync();
            }

            RecentLogs = await _db.AuditLogs.Include(a => a.User)
                                            .OrderByDescending(a => a.Timestamp)
                                            .Take(8).ToListAsync();
        }
    }
}
