using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentAppBackend.DTO.Groups;
using TournamentAppBackend.DTO.Matches;
using TournamentAppBackend.Services.Groups;

namespace TournamentAppBackend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _service;

        public GroupController(IGroupService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("tournaments/{tournamentId}/groups")]
        public async Task<IActionResult> Create(Guid tournamentId, [FromBody] CreateGroupDTO dto)
        {
            var result = await _service.CreateAsync(tournamentId, dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("tournaments/{tournamentId}/groups")]
        public async Task<IActionResult> GetByTournament(Guid tournamentId)
        {
            var result = await _service.GetByTournamentAsync(tournamentId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("groups/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("groups/{groupId}/matches")]
        public async Task<ActionResult<List<MatchResponseDTO>>> GetMatches(Guid groupId)
        {
            var matches = await _service.GetMatchesAsync(groupId);
            return Ok(matches);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPatch("groups/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("groups/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}