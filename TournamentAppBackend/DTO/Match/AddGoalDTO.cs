namespace TournamentAppBackend.DTO.Matches
{
    public class AddGoalDTO
    {
        public Guid TeamId { get; set; }
        public DateTime? ScoredAt { get; set; }
    }
}