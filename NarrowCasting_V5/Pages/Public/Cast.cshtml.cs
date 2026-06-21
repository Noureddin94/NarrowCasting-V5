using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Public
{
    public class CastModel : PageModel
    {
        private readonly IDepartmentService _departments;
        private readonly IScreenService _screens;

        public CastModel(IDepartmentService departments, IScreenService screens)
        {
            _departments = departments;
            _screens = screens;
        }

        public IEnumerable<Department> Departments { get; set; } = [];
        public IEnumerable<Screen> Screens { get; set; } = [];
        public string? SelectedDepartment { get; set; }

        public async Task OnGetAsync(string? dept)
        {
            Departments = await _departments.GetAllAsync();
            SelectedDepartment = dept;

            if (!string.IsNullOrEmpty(dept))
            {
                var department = Departments.FirstOrDefault(d => d.Name == dept);
                if (department is not null)
                    Screens = await _screens.GetByDepartmentAsync(department.Id);
            }
        }
    }
}
