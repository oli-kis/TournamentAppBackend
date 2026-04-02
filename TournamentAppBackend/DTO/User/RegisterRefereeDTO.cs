namespace TournamentAppBackend.DTO.User
{
    public class RegisterRefereeDTO
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}