namespace TournamentAppBackend.DTO.Teams
{
    public class TeamResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid GroupId { get; set; }
    }
}