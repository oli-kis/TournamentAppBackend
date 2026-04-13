using TournamentAppBackend.DTO.User;

namespace TournamentAppBackend.Services.User
{
    public interface IUserService
    {
        Task<UserResponseDTO> RegisterRefereeAsync(RegisterRefereeDTO dto);
        Task<UserResponseDTO> ApproveRefereeAsync(Guid id);
        Task<LoginResponseDTO> LoginAsync(LoginDTO dto);
        Task<UserResponseDTO> GetByIdAsync(Guid id);
        Task<List<UserResponseDTO>> GetAllUsersAsync();
        Task<List<UserResponseDTO>> GetAllRefereesAsync();
        Task<List<UserResponseDTO>> GetAllActiveRefereesAsync();
        Task<List<UserResponseDTO>> GetAllInactiveRefereesAsync();
        Task<List<UserResponseDTO>> GetAllPendingRefereesAsync();
        Task<UserResponseDTO> UpdateAsync(Guid id, UpdateUserDTO dto);
        Task DeleteAsync(Guid id);
        Task LogoutAsync(Guid id);
    }
}