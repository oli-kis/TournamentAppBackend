using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TournamentAppBackend.Services.Standings;

namespace TournamentAppBackend.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class StandingController : ControllerBase
    {
        private readonly IStandingService _service;

        public StandingController(IStandingService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet("groups/{groupId}/standings")]
        public async Task<IActionResult> GetByGroup(Guid groupId)
        {
            var result = await _service.GetByGroupAsync(groupId);
            return Ok(result);
        }
    }
}