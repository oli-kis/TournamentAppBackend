namespace TournamentAppBackend.Services.Knockout
{
    using Microsoft.EntityFrameworkCore;
    using TournamentAppBackend.DTO.Standings;
    using TournamentAppBackend.DTO.Tournaments;
    using TournamentAppBackend.Model;
    using TournamentAppBackend.Services.Standings;

    public class KnockoutService : IKnockoutService
    {
        private readonly AppDbContext _db;

        public KnockoutService(AppDbContext db)
        {
            _db = db;
        }

        public async Task StartKnockoutAsync(Guid tournamentId, int teamsPerGroup)
        {
            var groups = await _db.Groups.Where(g => g.TournamentId == tournamentId).ToListAsync();
            var teams = await _db.Teams.ToListAsync();

            var standingsService = new StandingService(_db);

            var allStandings = new List<(Guid groupId, List<StandingRowDTO> standings)>();

            foreach (var g in groups)
            {
                var s = await standingsService.GetByGroupAsync(g.Id);
                allStandings.Add((g.Id, s.Standings));
            }

            var qualified = new List<Team>();
            var nonQualified = new List<Team>();

            for (int pos = 0; pos < teamsPerGroup; pos++)
            {
                foreach (var g in allStandings)
                {
                    if (g.standings.Count > pos)
                    {
                        var teamId = g.standings[pos].TeamId;
                        qualified.Add(teams.First(t => t.Id == teamId));
                    }
                }
            }

            foreach (var g in allStandings)
            {
                for (int i = teamsPerGroup; i < g.standings.Count; i++)
                {
                    var teamId = g.standings[i].TeamId;
                    nonQualified.Add(teams.First(t => t.Id == teamId));
                }
            }

            var knockoutMatches = GenerateKnockout(qualified, tournamentId);
            var placementMatches = GeneratePlacement(nonQualified, tournamentId);

            _db.KnockoutMatches.AddRange(knockoutMatches);
            _db.PlacementMatches.AddRange(placementMatches);

            var tournament = await _db.Tournaments.FindAsync(tournamentId);
            tournament.KnockoutStarted = true;
            tournament.KnockoutTeamCount = qualified.Count;

            await _db.SaveChangesAsync();
        }

        public async Task UpdateKnockoutScoreAsync(Guid matchId, int homeScore, int awayScore)
        {
            var match = await _db.KnockoutMatches.FindAsync(matchId);
            if (match == null) throw new Exception("Match not found");

            match.HomeScore = homeScore;
            match.AwayScore = awayScore;
            match.Played = true;
            match.WinnerId = homeScore > awayScore ? match.HomeTeamId : match.AwayTeamId;

            await _db.SaveChangesAsync();
        }

        public async Task ResetKnockoutMatchAsync(Guid matchId)
        {
            var match = await _db.KnockoutMatches.FindAsync(matchId);
            if (match == null) return;

            match.HomeScore = null;
            match.AwayScore = null;
            match.Played = false;
            match.WinnerId = null;

            await _db.SaveChangesAsync();
        }

        public async Task UpdatePlacementScoreAsync(Guid matchId, int homeScore, int awayScore)
        {
            var match = await _db.PlacementMatches.FindAsync(matchId);
            if (match == null) return;

            match.HomeScore = homeScore;
            match.AwayScore = awayScore;
            match.Played = true;

            await _db.SaveChangesAsync();
        }

        public async Task ResetPlacementMatchAsync(Guid matchId)
        {
            var match = await _db.PlacementMatches.FindAsync(matchId);
            if (match == null) return;

            match.HomeScore = null;
            match.AwayScore = null;
            match.Played = false;

            await _db.SaveChangesAsync();
        }

        public async Task ResetTournamentAsync(Guid tournamentId)
        {
            var knockout = _db.KnockoutMatches.Where(m => m.TournamentId == tournamentId);
            var placement = _db.PlacementMatches.Where(m => m.TournamentId == tournamentId);

            _db.KnockoutMatches.RemoveRange(knockout);
            _db.PlacementMatches.RemoveRange(placement);

            var tournament = await _db.Tournaments.FindAsync(tournamentId);
            tournament.KnockoutStarted = false;
            tournament.KnockoutTeamCount = 0;

            await _db.SaveChangesAsync();
        }

        private List<KnockoutMatch> GenerateKnockout(List<Team> teams, Guid tournamentId)
        {
            var matches = new List<KnockoutMatch>();
            int round = teams.Count / 2;

            for (int i = 0; i < round; i++)
            {
                matches.Add(new KnockoutMatch
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    BracketRound = round,
                    Position = i,
                    HomeTeamId = teams[i].Id,
                    AwayTeamId = teams[teams.Count - 1 - i].Id
                });
            }

            return matches;
        }

        private List<PlacementMatch> GeneratePlacement(List<Team> teams, Guid tournamentId)
        {
            var matches = new List<PlacementMatch>();

            for (int i = 0; i < teams.Count / 2; i++)
            {
                matches.Add(new PlacementMatch
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    HomeTeamId = teams[i].Id,
                    AwayTeamId = teams[teams.Count - 1 - i].Id
                });
            }

            return matches;
        }
    }
}
