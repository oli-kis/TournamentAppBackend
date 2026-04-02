namespace TournamentAppBackend.DTO.Matches
{
    public class CreateMatchDTO
    {
        public Guid TournamentId { get; set; }
        public Guid GroupId { get; set; }
        public Guid HomeTeamId { get; set; }
        public Guid AwayTeamId { get; set; }
        public string Pitch { get; set; } = default!;
        public DateTime? ScheduledStart { get; set; }
    }
}