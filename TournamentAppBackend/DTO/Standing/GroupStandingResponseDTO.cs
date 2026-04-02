namespace TournamentAppBackend.DTO.Standings
{
    public class GroupStandingResponseDTO
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = default!;
        public List<StandingRowDTO> Standings { get; set; } = new();
    }
}