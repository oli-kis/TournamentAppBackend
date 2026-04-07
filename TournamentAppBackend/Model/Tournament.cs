namespace TournamentAppBackend.Model
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "DRAFT";
        public int Pitches { get; set; } = 1;
        public int MatchLengthInMinutes { get; set; } = 10;
        public int TransitionTime { get; set; } = 2;
        public bool KnockoutStarted { get; set; }
        public int KnockoutTeamCount { get; set; }
        public ICollection<KnockoutMatch> KnockoutMatches { get; set; } = new List<KnockoutMatch>();
        public ICollection<PlacementMatch> PlacementMatches { get; set; } = new List<PlacementMatch>();
    }
}