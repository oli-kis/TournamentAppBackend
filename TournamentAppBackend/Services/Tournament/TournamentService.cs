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
                Status = "DRAFT",
                Pitches = dto.Pitches,
                MatchLengthInMinutes = dto.MatchLengthInMinutes,
                TransitionTime = dto.TransitionTime,
                KnockoutStarted = false,
                KnockoutTeamCount = 0,
                KnockoutMatches = [],
                PlacementMatches = []
            };

            _db.Add(tournament);
            await _db.SaveChangesAsync();

            return Map(tournament);
        }

        public async Task<List<TournamentResponseDTO>> GetAllAsync()
        {
            return await _db.Set<Tournament>()
                .Include(t => t.KnockoutMatches)
                .Include(t => t.PlacementMatches)
                .Select(t => Map(t))
                .ToListAsync();
        }

        public async Task<TournamentResponseDTO> GetByIdAsync(Guid id)
        {
            var tournament = await _db.Set<Tournament>()
                .Include(t => t.KnockoutMatches)
                .Include(t => t.PlacementMatches)
                .FirstOrDefaultAsync(t => t.Id == id);

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
            if (dto.Pitches != null) tournament.Pitches = (int)dto.Pitches;
            if (dto.MatchLengthInMinutes != null) tournament.MatchLengthInMinutes = (int)dto.MatchLengthInMinutes;

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

        public async Task DeleteAllAsync()
        {
            var tournaments = await _db.Set<Tournament>().ToListAsync();

            if (tournaments.Count == 0)
                return;

            _db.RemoveRange(tournaments);
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
                Status = t.Status,
                Pitches = t.Pitches,
                MatchLengthInMinutes = t.MatchLengthInMinutes,
                TransitionTime = t.TransitionTime,
                KnockoutStarted = t.KnockoutStarted,
                KnockoutTeamCount = t.KnockoutTeamCount,
                KnockoutMatches = t.KnockoutMatches,
                PlacementMatches = t.PlacementMatches
            };
        }
    }
}