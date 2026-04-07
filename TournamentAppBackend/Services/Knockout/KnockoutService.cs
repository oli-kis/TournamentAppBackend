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
            var groups = await _db.Groups
                .Where(g => g.TournamentId == tournamentId)
                .ToListAsync();
            var teams = await _db.Teams.ToListAsync();

            var standingsService = new StandingService(_db);

            var allStandings = new List<(Guid groupId, List<StandingRowDTO> standings)>();

            foreach (var group in groups)
            {
                var result = await standingsService.GetByGroupAsync(group.Id);
                allStandings.Add((group.Id, result.Standings));
            }

            var (qualified, nonQualified) = GetQualifiedTeams(allStandings, teamsPerGroup, teams);

            var knockoutMatches = GenerateKnockoutBracket(qualified, tournamentId);

            var placementMatches = GeneratePlacementMatches(nonQualified, tournamentId);

            _db.KnockoutMatches.RemoveRange(
                _db.KnockoutMatches.Where(m => m.TournamentId == tournamentId));

            _db.PlacementMatches.RemoveRange(
                _db.PlacementMatches.Where(m => m.TournamentId == tournamentId));

            await _db.KnockoutMatches.AddRangeAsync(knockoutMatches);
            await _db.PlacementMatches.AddRangeAsync(placementMatches);

            var tournament = await _db.Tournaments.FindAsync(tournamentId);
            tournament.KnockoutStarted = true;
            tournament.KnockoutTeamCount = qualified.Count;

            await _db.SaveChangesAsync();
        }
        private (List<Team> qualified, List<Team> nonQualified)
GetQualifiedTeams(
    List<(Guid groupId, List<StandingRowDTO> standings)> allStandings,
    int teamsPerGroup,
    List<Team> allTeams)
        {
            var qualified = new List<Team>();
            var nonQualified = new List<Team>();

            // Interleave (1st of each group, then 2nd, etc.)
            for (int pos = 0; pos < teamsPerGroup; pos++)
            {
                foreach (var gs in allStandings)
                {
                    if (gs.standings.Count > pos)
                    {
                        var team = allTeams.FirstOrDefault(t => t.Id == gs.standings[pos].TeamId);
                        if (team != null) qualified.Add(team);
                    }
                }
            }

            // Non-qualified
            foreach (var gs in allStandings)
            {
                for (int pos = teamsPerGroup; pos < gs.standings.Count; pos++)
                {
                    var team = allTeams.FirstOrDefault(t => t.Id == gs.standings[pos].TeamId);
                    if (team != null) nonQualified.Add(team);
                }
            }

            return (qualified, nonQualified);
        }
        private List<KnockoutMatch> GenerateKnockoutBracket(List<Team> teams, Guid tournamentId)
        {
            int bracketSize = 2;
            while (bracketSize < teams.Count)
                bracketSize *= 2;

            var matches = new List<KnockoutMatch>();

            int currentRound = bracketSize / 2;

            // FIRST ROUND
            for (int i = 0; i < currentRound; i++)
            {
                var home = i < teams.Count ? teams[i] : null;
                var awayIdx = bracketSize - 1 - i;
                var away = awayIdx < teams.Count ? teams[awayIdx] : null;

                bool isBye = home == null || away == null;

                matches.Add(new KnockoutMatch
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    BracketRound = currentRound,
                    Position = i,
                    HomeTeamId = home?.Id,
                    AwayTeamId = away?.Id,
                    HomeScore = isBye ? (home != null ? 1 : 0) : null,
                    AwayScore = isBye ? (away != null ? 1 : 0) : null,
                    Played = isBye,
                    WinnerId = isBye ? (home?.Id ?? away?.Id) : null,
                    IsThirdPlace = false
                });
            }

            // NEXT ROUNDS
            currentRound /= 2;
            while (currentRound >= 1)
            {
                for (int i = 0; i < currentRound; i++)
                {
                    matches.Add(new KnockoutMatch
                    {
                        Id = Guid.NewGuid(),
                        TournamentId = tournamentId,
                        BracketRound = currentRound,
                        Position = i,
                        IsThirdPlace = false
                    });
                }

                currentRound /= 2;
            }

            // THIRD PLACE
            var semis = matches.Where(m => m.BracketRound == 2 && !m.IsThirdPlace).ToList();

            if (semis.Count == 2)
            {
                matches.Add(new KnockoutMatch
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    BracketRound = 1,
                    Position = 0,
                    IsThirdPlace = true
                });
            }

            return PropagateWinners(matches);
        }

        private List<KnockoutMatch> PropagateWinners(List<KnockoutMatch> matches)
        {
            var rounds = matches
                .Where(m => !m.IsThirdPlace)
                .Select(m => m.BracketRound)
                .Distinct()
                .OrderByDescending(r => r)
                .ToList();

            for (int r = 0; r < rounds.Count - 1; r++)
            {
                var currentRound = rounds[r];
                var nextRound = rounds[r + 1];

                var currentMatches = matches
                    .Where(m => m.BracketRound == currentRound && !m.IsThirdPlace)
                    .OrderBy(m => m.Position)
                    .ToList();

                foreach (var match in currentMatches)
                {
                    if (!match.Played || match.WinnerId == null) continue;

                    var nextPosition = match.Position / 2;

                    var nextMatch = matches.FirstOrDefault(m =>
                        m.BracketRound == nextRound &&
                        m.Position == nextPosition &&
                        !m.IsThirdPlace);

                    if (nextMatch == null) continue;

                    if (match.Position % 2 == 0)
                        nextMatch.HomeTeamId = match.WinnerId;
                    else
                        nextMatch.AwayTeamId = match.WinnerId;
                }
            }

            // THIRD PLACE
            var thirdPlace = matches.FirstOrDefault(m => m.IsThirdPlace);

            if (thirdPlace != null)
            {
                var semis = matches
                    .Where(m => m.BracketRound == 2 && !m.IsThirdPlace)
                    .OrderBy(m => m.Position)
                    .ToList();

                for (int i = 0; i < semis.Count; i++)
                {
                    var semi = semis[i];
                    if (!semi.Played || semi.WinnerId == null) continue;

                    var loser = semi.WinnerId == semi.HomeTeamId
                        ? semi.AwayTeamId
                        : semi.HomeTeamId;

                    if (i == 0) thirdPlace.HomeTeamId = loser;
                    else thirdPlace.AwayTeamId = loser;
                }
            }

            return matches;
        }
        private List<PlacementMatch> GeneratePlacementMatches(List<Team> teams, Guid tournamentId)
        {
            if (teams.Count < 2)
                return new List<PlacementMatch>();

            var list = new List<Team>(teams);

            if (list.Count % 2 != 0)
                list.Add(null); // BYE

            int n = list.Count;
            int rounds = n - 1;
            int matchesPerRound = n / 2;

            var matches = new List<PlacementMatch>();

            for (int round = 0; round < rounds; round++)
            {
                for (int match = 0; match < matchesPerRound; match++)
                {
                    int homeIdx = match == 0 ? 0 : ((round + match - 1) % (n - 1)) + 1;
                    int awayIdx = ((round + (n - 1) - match - 1) % (n - 1)) + 1;

                    var home = match == 0 ? list[0] : list[homeIdx];
                    var away = list[awayIdx];

                    if (home == null || away == null) continue;

                    matches.Add(new PlacementMatch
                    {
                        Id = Guid.NewGuid(),
                        TournamentId = tournamentId,
                        HomeTeamId = home.Id,
                        AwayTeamId = away.Id
                    });
                }
            }

            return matches;
        }
        public async Task UpdateKnockoutScoreAsync(Guid matchId, int homeScore, int awayScore)
        {
            var match = await _db.KnockoutMatches.FindAsync(matchId);
            if (match == null) throw new Exception("Match not found");

            match.HomeScore = homeScore;
            match.AwayScore = awayScore;
            match.Played = true;
            match.WinnerId = homeScore > awayScore ? match.HomeTeamId : match.AwayTeamId;

            var matches = await _db.KnockoutMatches
            .Where(m => m.TournamentId == match.TournamentId)
            .ToListAsync();

            matches = PropagateWinners(matches);

            // save updated matches
            _db.KnockoutMatches.UpdateRange(matches);

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
