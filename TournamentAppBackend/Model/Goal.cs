namespace TournamentAppBackend.Model
{
    public class Goal
    {
        public Guid Id { get; set; }

        public Guid MatchId { get; set; }
        public Match Match { get; set; } = default!;

        public Guid TeamId { get; set; }
        public Team Team { get; set; } = default!;

        public DateTime ScoredAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}