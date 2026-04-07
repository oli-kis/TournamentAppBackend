using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentAppBackend.DTO.Teams;
using TournamentAppBackend.Services.Teams;

namespace TournamentAppBackend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _service;

        public TeamController(ITeamService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("groups/{groupId}/teams")]
        public async Task<IActionResult> Create(Guid groupId, [FromBody] CreateTeamDTO dto)
        {
            var result = await _service.CreateAsync(groupId, dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("groups/{groupId}/teams")]
        public async Task<IActionResult> GetByGroup(Guid groupId)
        {
            var result = await _service.GetByGroupAsync(groupId);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("teams/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("search")]
        public async Task<ActionResult<List<TeamResponseDTO>>> Search([FromQuery] string name)
        {
            var result = await _service.SearchAsync(name);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPatch("teams/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeamDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("teams/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}