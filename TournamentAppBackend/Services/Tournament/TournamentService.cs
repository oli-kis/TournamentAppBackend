using Microsoft.EntityFrameworkCore;
using TournamentAppBackend.DTO.Tournaments;
using TournamentAppBackend.Model;

namespace TournamentAppBackend.Services.Tournaments
{
    public class TournamentService : ITournamentService
    {
        private readonly AppDbContext _db;

        public TournamentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<TournamentResponseDTO> CreateAsync(CreateTournamentDTO dto)
        {
            var tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = "DRAFT"
            };

            _db.Add(tournament);
            await _db.SaveChangesAsync();

            return Map(tournament);
        }

        public async Task<List<TournamentResponseDTO>> GetAllAsync()
        {
            return await _db.Set<Tournament>()
                .Select(t => Map(t))
                .ToListAsync();
        }

        public async Task<TournamentResponseDTO> GetByIdAsync(Guid id)
        {
            var tournament = await _db.Set<Tournament>().FindAsync(id);

            if (tournament == null)
                throw new Exception("Tournament not found");

            return Map(tournament);
        }

        public async Task<TournamentResponseDTO> UpdateAsync(Guid id, UpdateTournamentDTO dto)
        {
            var tournament = await _db.Set<Tournament>().FindAsync(id);

            if (tournament == null)
                throw new Exception("Tournament not found");

            if (dto.Name != null) tournament.Name = dto.Name;
            if (dto.Status != null) tournament.Status = dto.Status;

            await _db.SaveChangesAsync();

            return Map(tournament);
        }

        public async Task DeleteAsync(Guid id)
        {
            var tournament = await _db.Set<Tournament>().FindAsync(id);

            if (tournament == null)
                throw new Exception("Tournament not found");

            _db.Remove(tournament);
            await _db.SaveChangesAsync();
        }

        private static TournamentResponseDTO Map(Tournament t)
        {
            return new TournamentResponseDTO
            {
                Id = t.Id,
                Name = t.Name,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                Status = t.Status
            };
        }
    }
}