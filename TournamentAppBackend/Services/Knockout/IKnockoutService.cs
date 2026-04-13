using TournamentAppBackend.DTO.Tournaments;

namespace TournamentAppBackend.Services.Knockout
{
    public interface IKnockoutService
    {
        Task StartKnockoutAsync(Guid tournamentId, int teamsPerGroup);
        Task UpdateKnockoutScoreAsync(Guid matchId, int homeScore, int awayScore);
        Task ResetKnockoutMatchAsync(Guid matchId);
        Task UpdatePlacementScoreAsync(Guid matchId, int homeScore, int awayScore);
        Task ResetPlacementMatchAsync(Guid matchId);
        Task ResetTournamentAsync(Guid tournamentId);
    }
}
