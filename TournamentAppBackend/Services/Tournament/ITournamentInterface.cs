using TournamentAppBackend.DTO.Tournaments;

namespace TournamentAppBackend.Services.Tournaments
{
    public interface ITournamentService
    {
        Task<TournamentResponseDTO> CreateAsync(CreateTournamentDTO dto);
        Task<List<TournamentResponseDTO>> GetAllAsync();
        Task<TournamentResponseDTO> GetByIdAsync(Guid id);
        Task<TournamentResponseDTO> UpdateAsync(Guid id, UpdateTournamentDTO dto);
        Task DeleteAsync(Guid id);
        Task DeleteAllAsync();
    }
}