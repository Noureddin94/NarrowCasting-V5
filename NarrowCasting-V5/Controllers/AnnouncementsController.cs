using Microsoft.AspNetCore.Mvc;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementsController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _announcementService.GetAllAsync());
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return Ok(await _announcementService.GetActiveAsync());
        }

        [HttpGet("department/{departmentId:int}")]
        public async Task<IActionResult> GetDepartmentAnnouncements(int departmentId)
        {
            return Ok(await _announcementService.GetActiveForDepartmentAsync(departmentId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var announcement = await _announcementService.GetByIdAsync(id);

            if (announcement == null)
                return NotFound();

            return Ok(announcement);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Announcement announcement,
            [FromQuery] string userId)
        {
            await _announcementService.CreateAsync(announcement, userId);

            return CreatedAtAction(
                nameof(Get),
                new { id = announcement.Id },
                announcement);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            Announcement announcement,
            [FromQuery] string userId)
        {
            if (id != announcement.Id)
                return BadRequest();

            await _announcementService.UpdateAsync(announcement, userId);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromQuery] string userId)
        {
            await _announcementService.DeleteAsync(id, userId);

            return NoContent();
        }
    }
}
