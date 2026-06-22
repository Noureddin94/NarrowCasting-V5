using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departments;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentsController(IDepartmentService departments, UserManager<ApplicationUser> userManager)
        {
            _departments = departments;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _departments.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _departments.GetByIdAsync(id);
            if (department is null) return NotFound();
            return Ok(department);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Department department)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);
            if (userId is null) return Challenge();

            await _departments.CreateAsync(department, userId);

            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Department department)
        {
            if (id != department.Id) return BadRequest("Route id en body id komen niet overeen.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _departments.GetByIdAsync(id);
            if (existing is null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (userId is null) return Challenge();

            await _departments.UpdateAsync(department, userId);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _departments.GetByIdAsync(id);
            if (existing is null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (userId is null) return Challenge();

            await _departments.DeleteAsync(id, userId);

            return NoContent();
        }
    }
}
