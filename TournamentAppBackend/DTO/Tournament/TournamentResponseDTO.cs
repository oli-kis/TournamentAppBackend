namespace TournamentAppBackend.DTO.Tournaments
{
    public class TournamentResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = default!;
    }
}