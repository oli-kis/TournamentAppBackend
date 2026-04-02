using TournamentAppBackend.DTO.Standings;

namespace TournamentAppBackend.Services.Standings
{
    public interface IStandingService
    {
        Task<GroupStandingResponseDTO> GetByGroupAsync(Guid groupId);
    }
}