using Microsoft.AspNetCore.Mvc;
using TournamentAppBackend.DTO.Knockout;
using TournamentAppBackend.DTO.Match;
using TournamentAppBackend.Services.Knockout;

namespace TournamentAppBackend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class KnockoutController : ControllerBase
    {
        private readonly IKnockoutService _service;

        public KnockoutController(IKnockoutService service)
        {
            _service = service;
        }

        [HttpPost("tournaments/{id}/start-knockout")]
        public async Task<IActionResult> Start(Guid id, [FromBody] StartKnockoutDTO dto)
        {
            await _service.StartKnockoutAsync(id, dto.TeamsPerGroup);
            return Ok();
        }

        [HttpPost("knockout/{matchId}/score")]
        public async Task<IActionResult> UpdateScore(Guid matchId, [FromBody] UpdateScoreDTO dto)
        {
            await _service.UpdateKnockoutScoreAsync(matchId, dto.HomeScore, dto.AwayScore);
            return Ok();
        }

        [HttpPost("knockout/{matchId}/reset")]
        public async Task<IActionResult> Reset(Guid matchId)
        {
            await _service.ResetKnockoutMatchAsync(matchId);
            return Ok();
        }

        [HttpPost("placement/{matchId}/score")]
        public async Task<IActionResult> UpdatePlacement(Guid matchId, [FromBody] UpdateScoreDTO dto)
        {
            await _service.UpdatePlacementScoreAsync(matchId, dto.HomeScore, dto.AwayScore);
            return Ok();
        }

        [HttpPost("placement/{matchId}/reset")]
        public async Task<IActionResult> ResetPlacement(Guid matchId)
        {
            await _service.ResetPlacementMatchAsync(matchId);
            return Ok();
        }

        [HttpPost("tournaments/{id}/reset")]
        public async Task<IActionResult> ResetTournament(Guid id)
        {
            await _service.ResetTournamentAsync(id);
            return Ok();
        }
    }
}
