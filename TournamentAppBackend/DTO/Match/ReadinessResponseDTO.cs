namespace TournamentAppBackend.DTO.Matches
{
    public class ReadinessResponseDTO
    {
        public Guid MatchId { get; set; }
        public bool AllRefereesReady { get; set; }
        public string MatchStatus { get; set; } = default!;
        public List<RefereeAssignmentResponseDTO> Referees { get; set; } = new();
    }
}