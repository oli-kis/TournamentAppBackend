namespace TournamentAppBackend.Model
{
    public class Group
    {
        public Guid Id { get; set; }

        public Guid TournamentId { get; set; }
        public Tournament Tournament { get; set; } = default!;

        public string Name { get; set; } = default!;
    }
}