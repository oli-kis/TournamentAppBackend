namespace TournamentAppBackend.Model
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}