using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentAppBackend.DTO.Matches;
using TournamentAppBackend.Services.Matches;
using System.Security.Claims;

namespace TournamentAppBackend.Controllers
{
    [ApiController]
    [Route("api/v1/matches")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _service;

        public MatchController(IMatchService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchDTO dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMatchDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("{matchId}/referees")]
        public async Task<IActionResult> AssignReferees(Guid matchId, [FromBody] AssignRefereesDTO dto)
        {
            var result = await _service.AssignRefereesAsync(matchId, dto);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{matchId}/referees")]
        public async Task<IActionResult> GetReferees(Guid matchId)
        {
            var result = await _service.GetRefereesAsync(matchId);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN,REFEREE")]
        [HttpPost("{matchId}/readiness")]
        public async Task<IActionResult> SetReadiness(Guid matchId, [FromBody] SetReadinessDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized();

            var refereeId = Guid.Parse(userIdClaim);
            var result = await _service.SetReadinessAsync(matchId, refereeId, dto.Ready);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{matchId}/readiness")]
        public async Task<IActionResult> GetReadiness(Guid matchId)
        {
            var result = await _service.GetReadinessAsync(matchId);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN,REFEREE")]
        [HttpPost("{matchId}/start")]
        public async Task<IActionResult> Start(Guid matchId)
        {
            var result = await _service.StartMatchAsync(matchId);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN,REFEREE")]
        [HttpPost("{matchId}/goals")]
        public async Task<IActionResult> AddGoal(Guid matchId, [FromBody] AddGoalDTO dto)
        {
            var result = await _service.AddGoalAsync(matchId, dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{matchId}/goals")]
        public async Task<IActionResult> GetGoals(Guid matchId)
        {
            var result = await _service.GetGoalsAsync(matchId);
            return Ok(result);
        }

        [Authorize(Roles = "ADMIN,REFEREE")]
        [HttpDelete("{matchId}/goals/{goalId}")]
        public async Task<IActionResult> DeleteGoal(Guid matchId, Guid goalId)
        {
            await _service.DeleteGoalAsync(matchId, goalId);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN,REFEREE")]
        [HttpPost("{matchId}/finish")]
        public async Task<IActionResult> Finish(Guid matchId)
        {
            var result = await _service.FinishMatchAsync(matchId);
            return Ok(result);
        }

        [HttpPost("{matchId}/reset")]
        public async Task<IActionResult> Reset(Guid matchId)
        {
            await _service.ResetMatchAsync(matchId);
            return Ok();
        }
    }
}