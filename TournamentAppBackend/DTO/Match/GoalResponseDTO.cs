namespace TournamentAppBackend.DTO.Matches
{
    public class GoalResponseDTO
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = default!;
        public DateTime ScoredAt { get; set; }
    }
}