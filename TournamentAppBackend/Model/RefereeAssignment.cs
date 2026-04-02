namespace TournamentAppBackend.Model
{
    public class RefereeAssignment
    {
        public Guid Id { get; set; }

        public Guid MatchId { get; set; }
        public Match Match { get; set; } = default!;

        public Guid RefereeId { get; set; }
        public UserModel Referee { get; set; } = default!;

        public bool IsReady { get; set; }
        public DateTime? ReadyAt { get; set; }
    }
}