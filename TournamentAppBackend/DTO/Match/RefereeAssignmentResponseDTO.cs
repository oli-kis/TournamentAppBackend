namespace TournamentAppBackend.DTO.Matches
{
    public class RefereeAssignmentResponseDTO
    {
        public Guid RefereeId { get; set; }
        public string RefereeName { get; set; } = default!;
        public bool IsReady { get; set; }
        public DateTime? ReadyAt { get; set; }
    }
}