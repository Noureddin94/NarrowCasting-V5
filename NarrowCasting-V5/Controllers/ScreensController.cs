using Microsoft.AspNetCore.Mvc;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreensController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _screenService.GetAllAsync());
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return Ok(await _screenService.GetActiveAsync());
        }

        [HttpGet("department/{departmentId:int}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
        {
            return Ok(await _screenService.GetByDepartmentAsync(departmentId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var screen = await _screenService.GetByIdAsync(id);

            if (screen == null)
                return NotFound();

            return Ok(screen);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Screen screen,
            [FromQuery] string userId)
        {
            await _screenService.CreateAsync(screen, userId);

            return CreatedAtAction(
                nameof(Get),
                new { id = screen.Id },
                screen);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            Screen screen,
            [FromQuery] string userId)
        {
            if (id != screen.Id)
                return BadRequest();

            await _screenService.UpdateAsync(screen, userId);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromQuery] string userId)
        {
            await _screenService.DeleteAsync(id, userId);

            return NoContent();
        }
    }
}
