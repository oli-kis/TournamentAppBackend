namespace TournamentAppBackend.DTO.Matches
{
    public class MatchResponseDTO
    {
        public Guid Id { get; set; }
        public Guid TournamentId { get; set; }
        public Guid GroupId { get; set; }

        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = default!;

        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = default!;

        public string Pitch { get; set; } = default!;
        public DateTime? ScheduledStart { get; set; }

        public string Status { get; set; } = default!;
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

        public List<RefereeAssignmentResponseDTO> Referees { get; set; } = new();
    }
}