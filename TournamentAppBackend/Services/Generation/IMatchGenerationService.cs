using TournamentAppBackend.DTO.Generation;

namespace TournamentAppBackend.Services.Generation
{
    public interface IMatchGenerationService
    {
        Task<GenerateMatchesResponseDTO> GenerateForTournamentAsync(Guid tournamentId);
    }
}