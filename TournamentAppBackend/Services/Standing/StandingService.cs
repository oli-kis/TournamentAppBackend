using Microsoft.EntityFrameworkCore;
using TournamentAppBackend.DTO.Standings;
using TournamentAppBackend.Model;

namespace TournamentAppBackend.Services.Standings
{
    public class StandingService : IStandingService
    {
        private readonly AppDbContext _db;

        public StandingService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<GroupStandingResponseDTO> GetByGroupAsync(Guid groupId)
        {
            var group = await _db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                throw new Exception("Group not found");

            var teams = await _db.Teams
                .Where(t => t.GroupId == groupId)
                .ToListAsync();

            var finishedMatches = await _db.Matches
                .Include(m => m.Goals)
                .Where(m => m.GroupId == groupId && m.Status == "FINISHED")
                .ToListAsync();

            var rows = teams.Select(t => new StandingRowDTO
            {
                TeamId = t.Id,
                TeamName = t.Name,
                Played = 0,
                Won = 0,
                Drawn = 0,
                Lost = 0,
                GoalsFor = 0,
                GoalsAgainst = 0,
                GoalDifference = 0,
                Points = 0,
                Position = 0
            }).ToDictionary(x => x.TeamId);

            foreach (var match in finishedMatches)
            {
                var homeGoals = match.Goals.Count(g => !g.IsDeleted && g.TeamId == match.HomeTeamId);
                var awayGoals = match.Goals.Count(g => !g.IsDeleted && g.TeamId == match.AwayTeamId);

                var home = rows[match.HomeTeamId];
                var away = rows[match.AwayTeamId];

                home.Played++;
                away.Played++;

                home.GoalsFor += homeGoals;
                home.GoalsAgainst += awayGoals;

                away.GoalsFor += awayGoals;
                away.GoalsAgainst += homeGoals;

                if (homeGoals > awayGoals)
                {
                    home.Won++;
                    away.Lost++;
                    home.Points += 3;
                }
                else if (homeGoals < awayGoals)
                {
                    away.Won++;
                    home.Lost++;
                    away.Points += 3;
                }
                else
                {
                    home.Drawn++;
                    away.Drawn++;
                    home.Points += 1;
                    away.Points += 1;
                }
            }

            var ordered = rows.Values
                .Select(r =>
                {
                    r.GoalDifference = r.GoalsFor - r.GoalsAgainst;
                    return r;
                })
                .OrderByDescending(r => r.Points)
                .ThenByDescending(r => r.GoalDifference)
                .ThenByDescending(r => r.GoalsFor)
                .ThenBy(r => r.TeamName)
                .ToList();

            for (var i = 0; i < ordered.Count; i++)
            {
                ordered[i].Position = i + 1;
            }

            return new GroupStandingResponseDTO
            {
                GroupId = group.Id,
                GroupName = group.Name,
                Standings = ordered
            };
        }
    }
}