using Microsoft.EntityFrameworkCore;
using TournamentAppBackend.DTO.Groups;
using TournamentAppBackend.DTO.Matches;
using TournamentAppBackend.Model;

namespace TournamentAppBackend.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly AppDbContext _db;

        public GroupService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<GroupResponseDTO> CreateAsync(Guid tournamentId, CreateGroupDTO dto)
        {
            var tournamentExists = await _db.Tournaments
                .AnyAsync(t => t.Id == tournamentId);

            if (!tournamentExists)
                throw new Exception("Tournament not found");

            var group = new Group
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                Name = dto.Name
            };

            _db.Groups.Add(group);
            await _db.SaveChangesAsync();

            return Map(group);
        }

        public async Task<List<GroupResponseDTO>> GetByTournamentAsync(Guid tournamentId)
        {
            return await _db.Groups
                .Where(g => g.TournamentId == tournamentId)
                .Select(g => Map(g))
                .ToListAsync();
        }

        public async Task<GroupResponseDTO> GetByIdAsync(Guid id)
        {
            var group = await _db.Groups.FindAsync(id);

            if (group == null)
                throw new Exception("Group not found");

            return Map(group);
        }

        public async Task<List<MatchResponseDTO>> GetMatchesAsync(Guid groupId)
        {
            var groupExists = await _db.Groups
                .AnyAsync(g => g.Id == groupId);

            if (!groupExists)
                throw new Exception("Group not found");

            var matches = await _db.Matches
                .Where(m => m.GroupId == groupId)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.RefereeAssignments)
                    .ThenInclude(a => a.Referee)
                .Include(m => m.Goals)
                .ToListAsync();

            return matches.Select(MapMatch).ToList();
        }

        public async Task<GroupResponseDTO> UpdateAsync(Guid id, UpdateGroupDTO dto)
        {
            var group = await _db.Groups.FindAsync(id);

            if (group == null)
                throw new Exception("Group not found");

            if (dto.Name != null)
                group.Name = dto.Name;

            await _db.SaveChangesAsync();

            return Map(group);
        }

        public async Task DeleteAsync(Guid id)
        {
            var group = await _db.Groups.FindAsync(id);

            if (group == null)
                throw new Exception("Group not found");

            _db.Groups.Remove(group);
            await _db.SaveChangesAsync();
        }

        private static GroupResponseDTO Map(Group g)
        {
            return new GroupResponseDTO
            {
                Id = g.Id,
                Name = g.Name,
                TournamentId = g.TournamentId
            };
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