using TournamentAppBackend.DTO.Groups;

namespace TournamentAppBackend.Services.Groups
{
    public interface IGroupService
    {
        Task<GroupResponseDTO> CreateAsync(Guid tournamentId, CreateGroupDTO dto);
        Task<List<GroupResponseDTO>> GetByTournamentAsync(Guid tournamentId);
        Task<GroupResponseDTO> GetByIdAsync(Guid id);
        Task<GroupResponseDTO> UpdateAsync(Guid id, UpdateGroupDTO dto);
        Task DeleteAsync(Guid id);
    }
}