using TournamentAppBackend.DTO.Matches;

namespace TournamentAppBackend.Services.Matches
{
    public interface IMatchService
    {
        Task<MatchResponseDTO> CreateAsync(CreateMatchDTO dto);
        Task<List<MatchResponseDTO>> GetAllAsync();
        Task<MatchResponseDTO> GetByIdAsync(Guid id);
        Task<MatchResponseDTO> UpdateAsync(Guid id, UpdateMatchDTO dto);
        Task DeleteAsync(Guid id);

        Task<List<RefereeAssignmentResponseDTO>> AssignRefereesAsync(Guid matchId, AssignRefereesDTO dto);
        Task<List<RefereeAssignmentResponseDTO>> GetRefereesAsync(Guid matchId);

        Task<ReadinessResponseDTO> SetReadinessAsync(Guid matchId, Guid refereeId, bool ready);
        Task<ReadinessResponseDTO> GetReadinessAsync(Guid matchId);

        Task<MatchResponseDTO> StartMatchAsync(Guid matchId);
        Task<GoalResponseDTO> AddGoalAsync(Guid matchId, AddGoalDTO dto);
        Task<List<GoalResponseDTO>> GetGoalsAsync(Guid matchId);
        Task DeleteGoalAsync(Guid matchId, Guid goalId);
        Task<MatchResponseDTO> FinishMatchAsync(Guid matchId);
        Task ResetMatchAsync(Guid matchId);

    }
}