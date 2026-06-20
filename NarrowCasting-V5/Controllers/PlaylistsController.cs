using Microsoft.AspNetCore.Mvc;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _playlistService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var playlist = await _playlistService.GetByIdAsync(id);

            if (playlist == null)
                return NotFound();

            return Ok(playlist);
        }

        [HttpGet("screen/{screenId:int}")]
        public async Task<IActionResult> GetByScreen(int screenId)
        {
            var playlist = await _playlistService.GetByScreenAsync(screenId);

            if (playlist == null)
                return NotFound();

            return Ok(playlist);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Playlist playlist,
            [FromQuery] string userId)
        {
            await _playlistService.CreateAsync(playlist, userId);

            return CreatedAtAction(
                nameof(Get),
                new { id = playlist.Id },
                playlist);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            Playlist playlist,
            [FromQuery] string userId)
        {
            if (id != playlist.Id)
                return BadRequest();

            await _playlistService.UpdateAsync(playlist, userId);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id,
            [FromQuery] string userId)
        {
            await _playlistService.DeleteAsync(id, userId);

            return NoContent();
        }

        [HttpPost("{playlistId:int}/items")]
        public async Task<IActionResult> AddItem(
            int playlistId,
            PlaylistItem item,
            [FromQuery] string userId)
        {
            await _playlistService.AddItemAsync(playlistId, item, userId);

            return Ok();
        }

        [HttpDelete("items/{itemId:int}")]
        public async Task<IActionResult> RemoveItem(
            int itemId,
            [FromQuery] string userId)
        {
            await _playlistService.RemoveItemAsync(itemId, userId);

            return NoContent();
        }

    }
}
