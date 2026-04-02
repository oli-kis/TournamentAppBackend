namespace TournamentAppBackend.DTO.User
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}
