namespace TournamentAppBackend.Model
{
    public class KnockoutMatch
    {
        public Guid Id { get; set; }
        public Guid TournamentId { get; set; }

        public int BracketRound { get; set; }
        public int Position { get; set; }

        public Guid? HomeTeamId { get; set; }
        public Guid? AwayTeamId { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public bool Played { get; set; }
        public Guid? WinnerId { get; set; }

        public bool IsThirdPlace { get; set; }
    }
}
