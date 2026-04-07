using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentAppBackend.Services.Generation;

namespace TournamentAppBackend.Controllers
{
    [ApiController]
    [Route("api/v1/tournaments")]
    public class MatchGenerationController : ControllerBase
    {
        private readonly IMatchGenerationService _service;

        public MatchGenerationController(IMatchGenerationService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("{tournamentId}/generate-matches")]
        public async Task<IActionResult> Generate(Guid tournamentId)
        {
            var result = await _service.GenerateForTournamentAsync(tournamentId);
            return Ok(result);
        }
    }
}