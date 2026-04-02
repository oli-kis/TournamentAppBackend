using TournamentAppBackend.DTO.Teams;

namespace TournamentAppBackend.Services.Teams
{
    public interface ITeamService
    {
        Task<TeamResponseDTO> CreateAsync(Guid groupId, CreateTeamDTO dto);
        Task<List<TeamResponseDTO>> GetByGroupAsync(Guid groupId);
        Task<TeamResponseDTO> GetByIdAsync(Guid id);
        Task<TeamResponseDTO> UpdateAsync(Guid id, UpdateTeamDTO dto);
        Task DeleteAsync(Guid id);
    }
}