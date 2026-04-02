using Microsoft.EntityFrameworkCore;
using TournamentAppBackend.Model;

namespace TournamentAppBackend
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users => Set<UserModel>();
        public DbSet<Tournament> Tournaments => Set<Tournament>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<RefereeAssignment> RefereeAssignments => Set<RefereeAssignment>();
        public DbSet<Goal> Goals => Set<Goal>();
        public DbSet<KnockoutMatch> KnockoutMatches => Set<KnockoutMatch>();
        public DbSet<PlacementMatch> PlacementMatches => Set<PlacementMatch>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany()
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany()
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RefereeAssignment>()
                .HasIndex(a => new { a.MatchId, a.RefereeId })
                .IsUnique();
        }
    }
}