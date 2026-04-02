using Microsoft.EntityFrameworkCore;
using TournamentAppBackend.DTO.Groups;
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
    }
}