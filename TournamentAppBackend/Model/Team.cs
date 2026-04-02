namespace TournamentAppBackend.Model
{
    public class Team
    {
        public Guid Id { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; } = default!;

        public string Name { get; set; } = default!;
    }
}