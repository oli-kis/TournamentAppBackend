using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentAppBackend.DTO.Groups;
using TournamentAppBackend.Services.Groups;

namespace TournamentAppBackend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(Roles = "ADMIN")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _service;

        public GroupController(IGroupService service)
        {
            _service = service;
        }

        [HttpPost("tournaments/{tournamentId}/groups")]
        public async Task<IActionResult> Create(Guid tournamentId, [FromBody] CreateGroupDTO dto)
        {
            var result = await _service.CreateAsync(tournamentId, dto);
            return Ok(result);
        }

        [HttpGet("tournaments/{tournamentId}/groups")]
        public async Task<IActionResult> GetByTournament(Guid tournamentId)
        {
            var result = await _service.GetByTournamentAsync(tournamentId);
            return Ok(result);
        }

        [HttpGet("groups/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPatch("groups/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("groups/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}