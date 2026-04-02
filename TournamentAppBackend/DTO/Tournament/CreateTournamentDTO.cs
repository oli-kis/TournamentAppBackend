namespace TournamentAppBackend.DTO.Tournaments
{
    public class CreateTournamentDTO
    {
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}