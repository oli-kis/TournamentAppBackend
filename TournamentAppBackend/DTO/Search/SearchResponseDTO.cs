namespace TournamentAppBackend.DTO.Search
{
    public class SearchResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid GroupId { get; set; }
        public string GroupName { get; set; }

        public Guid TournamentId { get; set; }
        public string TournamentName { get; set; }
    }
}
