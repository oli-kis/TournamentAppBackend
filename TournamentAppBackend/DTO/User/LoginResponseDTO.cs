namespace TournamentAppBackend.DTO.User
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } = default!;
        public UserResponseDTO User { get; set; } = default!;
    }
}