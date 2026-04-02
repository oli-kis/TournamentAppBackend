namespace TournamentAppBackend.DTO.Standings
{
    public class StandingRowDTO
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = default!;
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
    }
}