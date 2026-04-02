using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentAppBackend.DTO.Teams;
using TournamentAppBackend.Services.Teams;

namespace TournamentAppBackend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(Roles = "ADMIN")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _service;

        public TeamController(ITeamService service)
        {
            _service = service;
        }

        [HttpPost("groups/{groupId}/teams")]
        public async Task<IActionResult> Create(Guid groupId, [FromBody] CreateTeamDTO dto)
        {
            var result = await _service.CreateAsync(groupId, dto);
            return Ok(result);
        }

        [HttpGet("groups/{groupId}/teams")]
        public async Task<IActionResult> GetByGroup(Guid groupId)
        {
            var result = await _service.GetByGroupAsync(groupId);
            return Ok(result);
        }

        [HttpGet("teams/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPatch("teams/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeamDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("teams/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}