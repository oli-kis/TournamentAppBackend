namespace TournamentAppBackend.Model
{
    public class Match
    {
        public Guid Id { get; set; }

        public Guid TournamentId { get; set; }
        public Tournament Tournament { get; set; } = default!;

        public Guid GroupId { get; set; }
        public Group Group { get; set; } = default!;

        public Guid HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = default!;

        public Guid AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = default!;

        public string Pitch { get; set; } = default!;
        public DateTime? ScheduledStart { get; set; }

        public string Status { get; set; } = "SCHEDULED";

        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public ICollection<RefereeAssignment> RefereeAssignments { get; set; } = new List<RefereeAssignment>();
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    }
}