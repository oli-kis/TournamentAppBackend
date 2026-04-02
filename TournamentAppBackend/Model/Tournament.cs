namespace TournamentAppBackend.Model
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "DRAFT";
    }
}