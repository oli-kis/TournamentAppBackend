namespace TournamentAppBackend.DTO.Groups
{
    public class GroupResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid TournamentId { get; set; }
    }
}