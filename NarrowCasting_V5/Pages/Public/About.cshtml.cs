using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Public
{
    public class AboutModel : PageModel
    {
        private readonly IDepartmentService _departments;
        public AboutModel(IDepartmentService departments) => _departments = departments;

        public IEnumerable<Department> Departments { get; set; } = [];

        public async Task OnGetAsync() =>
            Departments = await _departments.GetAllAsync();
    }
}
