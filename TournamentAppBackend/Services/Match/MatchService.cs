using Microsoft.EntityFrameworkCore;
using TournamentAppBackend.DTO.Matches;
using TournamentAppBackend.Model;

namespace TournamentAppBackend.Services.Matches
{
    public class MatchService : IMatchService
    {
        private readonly AppDbContext _db;

        public MatchService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<MatchResponseDTO> CreateAsync(CreateMatchDTO dto)
        {
            if (dto.HomeTeamId == dto.AwayTeamId)
                throw new Exception("Home team and away team must be different");

            var tournamentExists = await _db.Tournaments.AnyAsync(t => t.Id == dto.TournamentId);
            if (!tournamentExists)
                throw new Exception("Tournament not found");

            var groupExists = await _db.Groups.AnyAsync(g => g.Id == dto.GroupId && g.TournamentId == dto.TournamentId);
            if (!groupExists)
                throw new Exception("Group not found");

            var homeTeam = await _db.Teams.FirstOrDefaultAsync(t => t.Id == dto.HomeTeamId && t.GroupId == dto.GroupId);
            var awayTeam = await _db.Teams.FirstOrDefaultAsync(t => t.Id == dto.AwayTeamId && t.GroupId == dto.GroupId);

            if (homeTeam == null || awayTeam == null)
                throw new Exception("Both teams must exist in the given group");

            var match = new Match
            {
                Id = Guid.NewGuid(),
                TournamentId = dto.TournamentId,
                GroupId = dto.GroupId,
                HomeTeamId = dto.HomeTeamId,
                AwayTeamId = dto.AwayTeamId,
                Pitch = dto.Pitch,
                ScheduledStart = dto.ScheduledStart,
                Status = "SCHEDULED"
            };

            _db.Matches.Add(match);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(match.Id);
        }

        public async Task<List<MatchResponseDTO>> GetAllAsync()
        {
            var matches = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.RefereeAssignments)
                    .ThenInclude(a => a.Referee)
                .Include(m => m.Goals)
                .ToListAsync();

            return matches.Select(MapMatch).ToList();
        }

        public async Task<MatchResponseDTO> GetByIdAsync(Guid id)
        {
            var match = await _db.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.RefereeAssignments)
                    .ThenInclude(a => a.Referee)
                .Include(m => m.Goals)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                throw new Exception("Match not found");

            return MapMatch(match);
        }

        public async Task<MatchResponseDTO> UpdateAsync(Guid id, UpdateMatchDTO dto)
        {
            var match = await _db.Matches.FindAsync(id);

            if (match == null)
                throw new Exception("Match not found");

            if (dto.Pitch != null)
                match.Pitch = dto.Pitch;

            if (dto.ScheduledStart.HasValue)
                match.ScheduledStart = dto.ScheduledStart.Value;

            if (dto.Status != null)
                match.Status = dto.Status;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var match = await _db.Matches.FindAsync(id);

            if (match == null)
                throw new Exception("Match not found");

            _db.Matches.Remove(match);
            await _db.SaveChangesAsync();
        }

        public async Task<List<RefereeAssignmentResponseDTO>> AssignRefereesAsync(Guid matchId, AssignRefereesDTO dto)
        {
            var match = await _db.Matches
                .Include(m => m.RefereeAssignments)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                throw new Exception("Match not found");

            if (match.Status == "LIVE" || match.Status == "FINISHED")
                throw new Exception("Cannot change referees after match started");

            var refereeIds = dto.RefereeIds.Distinct().ToList();

            var referees = await _db.Users
                .Where(u => refereeIds.Contains(u.Id) && u.Role == "REFEREE" && u.Status == "ACTIVE")
                .ToListAsync();

            if (referees.Count != refereeIds.Count)
                throw new Exception("One or more referees are invalid");

            _db.RefereeAssignments.RemoveRange(match.RefereeAssignments);

            var assignments = referees.Select(r => new RefereeAssignment
            {
                Id = Guid.NewGuid(),
                MatchId = matchId,
                RefereeId = r.Id,
                IsReady = false,
                ReadyAt = null
            }).ToList();

            _db.RefereeAssignments.AddRange(assignments);

            match.Status = assignments.Count > 0 ? "WAITING_FOR_REFEREES" : "SCHEDULED";

            await _db.SaveChangesAsync();

            return await GetRefereesAsync(matchId);
        }

        public async Task<List<RefereeAssignmentResponseDTO>> GetRefereesAsync(Guid matchId)
        {
            var exists = await _db.Matches.AnyAsync(m => m.Id == matchId);
            if (!exists)
                throw new Exception("Match not found");

            var assignments = await _db.RefereeAssignments
                .Include(a => a.Referee)
                .Where(a => a.MatchId == matchId)
                .ToListAsync();

            return assignments.Select(MapAssignment).ToList();
        }

        public async Task<ReadinessResponseDTO> SetReadinessAsync(Guid matchId, Guid refereeId, bool ready)
        {
            var match = await _db.Matches
                .Include(m => m.RefereeAssignments)
                    .ThenInclude(a => a.Referee)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                throw new Exception("Match not found");

            if (match.Status == "LIVE" || match.Status == "FINISHED")
                throw new Exception("Cannot change readiness after match started");

            var assignment = match.RefereeAssignments.FirstOrDefault(a => a.RefereeId == refereeId);
            if (assignment == null)
                throw new Exception("Referee is not assigned to this match");

            assignment.IsReady = ready;
            assignment.ReadyAt = ready ? DateTime.UtcNow : null;

            var allReady = match.RefereeAssignments.Count > 0 && match.RefereeAssignments.All(a => a.IsReady);
            match.Status = allReady ? "READY" : "WAITING_FOR_REFEREES";

            await _db.SaveChangesAsync();

            return new ReadinessResponseDTO
            {
                MatchId = match.Id,
                AllRefereesReady = allReady,
                MatchStatus = match.Status,
                Referees = match.RefereeAssignments.Select(MapAssignment).ToList()
            };
        }

        public async Task<ReadinessResponseDTO> GetReadinessAsync(Guid matchId)
        {
            var match = await _db.Matches
                .Include(m => m.RefereeAssignments)
                    .ThenInclude(a => a.Referee)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                throw new Exception("Match not found");

            var allReady = match.RefereeAssignments.Count > 0 && match.RefereeAssignments.All(a => a.IsReady);

            return new ReadinessResponseDTO
            {
                MatchId = match.Id,
                AllRefereesReady = allReady,
                MatchStatus = match.Status,
                Referees = match.RefereeAssignments.Select(MapAssignment).ToList()
            };
        }

        public async Task<MatchResponseDTO> StartMatchAsync(Guid matchId)
        {
            var match = await _db.Matches
                .Include(m => m.RefereeAssignments)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                throw new Exception("Match not found");

            if (match.Status != "READY")
                throw new Exception("Match is not ready");

            var allReady = match.RefereeAssignments.Count > 0 && match.RefereeAssignments.All(a => a.IsReady);
            if (!allReady)
                throw new Exception("Not all referees are ready");

            match.Status = "LIVE";
            match.StartedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await GetByIdAsync(matchId);
        }

        public async Task<GoalResponseDTO> AddGoalAsync(Guid matchId, AddGoalDTO dto)
        {
            var match = await _db.Matches
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                throw new Exception("Match not found");

            if (match.Status != "LIVE")
                throw new Exception("Goals can only be added while match is live");

            if (dto.TeamId != match.HomeTeamId && dto.TeamId != match.AwayTeamId)
                throw new Exception("Team does not belong to this match");

            var team = await _db.Teams.FirstAsync(t => t.Id == dto.TeamId);

            var goal = new Goal
            {
                Id = Guid.NewGuid(),
                MatchId = matchId,
                TeamId = dto.TeamId,
                ScoredAt = dto.ScoredAt ?? DateTime.UtcNow,
                IsDeleted = false
            };

            _db.Goals.Add(goal);
            await _db.SaveChangesAsync();

            return new GoalResponseDTO
            {
                Id = goal.Id,
                TeamId = goal.TeamId,
                TeamName = team.Name,
                ScoredAt = goal.ScoredAt
            };
        }

        public async Task<List<GoalResponseDTO>> GetGoalsAsync(Guid matchId)
        {
            var exists = await _db.Matches.AnyAsync(m => m.Id == matchId);
            if (!exists)
                throw new Exception("Match not found");

            var goals = await _db.Goals
                .Include(g => g.Team)
                .Where(g => g.MatchId == matchId && !g.IsDeleted)
                .OrderBy(g => g.ScoredAt)
                .ToListAsync();

            return goals.Select(g => new GoalResponseDTO
            {
                Id = g.Id,
                TeamId = g.TeamId,
                TeamName = g.Team.Name,
                ScoredAt = g.ScoredAt
            }).ToList();
        }

        public async Task DeleteGoalAsync(Guid matchId, Guid goalId)
        {
            var goal = await _db.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.MatchId == matchId);

            if (goal == null)
                throw new Exception("Goal not found");

            goal.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<MatchResponseDTO> FinishMatchAsync(Guid matchId)
        {
            var match = await _db.Matches.FindAsync(matchId);

            if (match == null)
                throw new Exception("Match not found");

            if (match.Status != "LIVE")
                throw new Exception("Only live matches can be finished");

            match.Status = "FINISHED";
            match.FinishedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await GetByIdAsync(matchId);
        }

        private static RefereeAssignmentResponseDTO MapAssignment(RefereeAssignment a)
        {
            return new RefereeAssignmentResponseDTO
            {
                RefereeId = a.RefereeId,
                RefereeName = a.Referee.Name,
                IsReady = a.IsReady,
                ReadyAt = a.ReadyAt
            };
        }

        private static MatchResponseDTO MapMatch(Match m)
        {
            var homeScore = m.Goals.Count(g => !g.IsDeleted && g.TeamId == m.HomeTeamId);
            var awayScore = m.Goals.Count(g => !g.IsDeleted && g.TeamId == m.AwayTeamId);

            return new MatchResponseDTO
            {
                Id = m.Id,
                TournamentId = m.TournamentId,
                GroupId = m.GroupId,
                HomeTeamId = m.HomeTeamId,
                HomeTeamName = m.HomeTeam.Name,
                AwayTeamId = m.AwayTeamId,
                AwayTeamName = m.AwayTeam.Name,
                Pitch = m.Pitch,
                ScheduledStart = m.ScheduledStart,
                Status = m.Status,
                StartedAt = m.StartedAt,
                FinishedAt = m.FinishedAt,
                HomeScore = homeScore,
                AwayScore = awayScore,
                Referees = m.RefereeAssignments.Select(MapAssignment).ToList()
            };
        }
    }
}