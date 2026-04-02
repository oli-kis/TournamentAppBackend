namespace TournamentAppBackend.DTO.Matches
{
    public class UpdateMatchDTO
    {
        public string? Pitch { get; set; }
        public DateTime? ScheduledStart { get; set; }
        public string? Status { get; set; }
    }
}