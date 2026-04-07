namespace TournamentAppBackend.DTO.Tournaments
{
    public class CreateTournamentDTO
    {
        public string Name { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Pitches { get; set; }
        public int MatchLengthInMinutes { get; set; }
        public int TransitionTime { get; set; }
    }
}