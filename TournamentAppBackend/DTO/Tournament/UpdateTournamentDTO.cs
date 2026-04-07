namespace TournamentAppBackend.DTO.Tournaments
{
    public class UpdateTournamentDTO
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public int? Pitches { get; set; }
        public int? MatchLengthInMinutes { get; set; }
        public int? TransitionTime { get; set; }
    }
}