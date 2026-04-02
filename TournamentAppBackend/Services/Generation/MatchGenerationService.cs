using Microsoft.EntityFrameworkCore;
using TournamentAppBackend.DTO.Generation;
using TournamentAppBackend.Model;
using TournamentAppBackend.Services.Generation;

namespace TournamentAppBackend.Services.Matches
{
    public class MatchGenerationService : IMatchGenerationService
    {
        private readonly AppDbContext _db;

        public MatchGenerationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<GenerateMatchesResponseDTO> GenerateForTournamentAsync(Guid tournamentId)
        {
            var tournament = await _db.Tournaments
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
                throw new Exception("Tournament not found");

            var groups = await _db.Groups
                .Where(g => g.TournamentId == tournamentId)
                .ToListAsync();

            var allMatches = new List<Match>();

            foreach (var group in groups)
            {
                var teams = await _db.Teams
                    .Where(t => t.GroupId == group.Id)
                    .ToListAsync();

                if (teams.Count < 2)
                    continue;

                var groupMatches = GenerateRoundRobinForGroup(teams, group.Id, tournament.Id);
                allMatches.AddRange(groupMatches);
            }

            _db.Matches.AddRange(allMatches);
            await _db.SaveChangesAsync();

            return new GenerateMatchesResponseDTO
            {
                MatchesCreated = allMatches.Count
            };
        }

        private List<Match> GenerateRoundRobinForGroup(List<Team> teams, Guid groupId, Guid tournamentId)
        {
            var matches = new List<Match>();
            var teamList = new List<Team>(teams);

            if (teamList.Count % 2 != 0)
            {
                teamList.Add(new Team
                {
                    Id = Guid.Empty, // represents BYE
                    Name = "BYE"
                });
            }

            var n = teamList.Count;
            var rounds = n - 1;
            var matchesPerRound = n / 2;

            for (int round = 0; round < rounds; round++)
            {
                for (int match = 0; match < matchesPerRound; match++)
                {
                    var homeIndex = match == 0 ? 0 : ((round + match - 1) % (n - 1)) + 1;
                    var awayIndex = ((round + (n - 1) - match - 1) % (n - 1)) + 1;

                    var homeTeam = match == 0 ? teamList[0] : teamList[homeIndex];
                    var awayTeam = teamList[awayIndex];

                    if (homeTeam.Id == Guid.Empty || awayTeam.Id == Guid.Empty)
                        continue;

                    matches.Add(new Match
                    {
                        Id = Guid.NewGuid(),
                        TournamentId = tournamentId,
                        GroupId = groupId,
                        HomeTeamId = homeTeam.Id,
                        AwayTeamId = awayTeam.Id,
                        Status = "SCHEDULED",
                        Pitch = "defaultPitch"
                    });
                }
            }

            return matches;
        }
    }
}