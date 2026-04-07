using Microsoft.EntityFrameworkCore;
using TournamentAppBackend.DTO.Teams;
using TournamentAppBackend.DTO.Search;
using TournamentAppBackend.Model;

namespace TournamentAppBackend.Services.Teams
{
    public class TeamService : ITeamService
    {
        private readonly AppDbContext _db;

        public TeamService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TeamResponseDTO> CreateAsync(Guid groupId, CreateTeamDTO dto)
        {
            var groupExists = await _db.Groups
                .AnyAsync(g => g.Id == groupId);

            if (!groupExists)
                throw new Exception("Group not found");

            var team = new Team
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                Name = dto.Name
            };

            _db.Teams.Add(team);
            await _db.SaveChangesAsync();

            return Map(team);
        }

        public async Task<List<TeamResponseDTO>> GetByGroupAsync(Guid groupId)
        {
            return await _db.Teams
                .Where(t => t.GroupId == groupId)
                .Select(t => Map(t))
                .ToListAsync();
        }

        public async Task<TeamResponseDTO> GetByIdAsync(Guid id)
        {
            var team = await _db.Teams.FindAsync(id);

            if (team == null)
                throw new Exception("Team not found");

            return Map(team);
        }

        public async Task<List<SearchResponseDTO>> SearchAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<SearchResponseDTO>();

            return await _db.Teams
                .Where(t => t.Name.Contains(name))
                .Select(t => new SearchResponseDTO
                {
                    Id = t.Id,
                    Name = t.Name,

                    GroupId = t.GroupId,
                    GroupName = t.Group.Name,

                    TournamentId = t.Group.TournamentId,
                    TournamentName = t.Group.Tournament.Name
                })
                .ToListAsync();
        }

        public async Task<TeamResponseDTO> UpdateAsync(Guid id, UpdateTeamDTO dto)
        {
            var team = await _db.Teams.FindAsync(id);

            if (team == null)
                throw new Exception("Team not found");

            if (dto.Name != null)
                team.Name = dto.Name;

            await _db.SaveChangesAsync();

            return Map(team);
        }

        public async Task DeleteAsync(Guid id)
        {
            var team = await _db.Teams.FindAsync(id);

            if (team == null)
                throw new Exception("Team not found");

            _db.Teams.Remove(team);
            await _db.SaveChangesAsync();
        }

        private static TeamResponseDTO Map(Team t)
        {
            return new TeamResponseDTO
            {
                Id = t.Id,
                Name = t.Name,
                GroupId = t.GroupId
            };
        }
    }
}