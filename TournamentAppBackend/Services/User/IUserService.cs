using TournamentAppBackend.DTO.User;

namespace TournamentAppBackend.Services.User
{
    public interface IUserService
    {
        Task<UserResponseDTO> RegisterRefereeAsync(RegisterRefereeDTO dto);
        Task<LoginResponseDTO> LoginAsync(LoginDTO dto);
        Task<UserResponseDTO> GetByIdAsync(Guid id);
        Task<List<UserResponseDTO>> GetAllRefereesAsync();
        Task<List<UserResponseDTO>> GetAllActiveRefereesAsync();
        Task<UserResponseDTO> UpdateAsync(Guid id, UpdateUserDTO dto);
        Task DeleteAsync(Guid id);
        Task LogoutAsync(Guid id);
    }
}