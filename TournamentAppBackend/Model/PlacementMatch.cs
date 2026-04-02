namespace TournamentAppBackend.Model
{
    public class PlacementMatch
    {
        public Guid Id { get; set; }
        public Guid TournamentId { get; set; }

        public Guid HomeTeamId { get; set; }
        public Guid AwayTeamId { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public bool Played { get; set; }
    }
}
