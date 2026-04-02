using TournamentAppBackend.Model;

namespace TournamentAppBackend.DTO.Tournaments
{
    public class TournamentResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = default!;
        public bool KnockoutStarted { get; set; }
        public int KnockoutTeamCount { get; set; }

        public ICollection<KnockoutMatch> KnockoutMatches { get; set; } = new List<KnockoutMatch>();
        public ICollection<PlacementMatch> PlacementMatches { get; set; } = new List<PlacementMatch>();
    }
}