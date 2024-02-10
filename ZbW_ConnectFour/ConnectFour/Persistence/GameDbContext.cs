using ConnectFour.Models;
using Microsoft.EntityFrameworkCore;

public class GameDbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Robot> Robots { get; set; }

    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User zu Player (1-zu-1)
        modelBuilder.Entity<Player>()
            .HasOne(p => p.User)
            .WithOne(u => u.Player)
            .HasForeignKey<Player>(p => p.UserId);

        // Konfiguriere Game-Entity
            modelBuilder.Entity<Game>()
            .HasOne(g => g.PlayerOne)
            .WithMany() // Ohne Navigationseigenschaft in Player
            .HasForeignKey("PlayerOneId");

        modelBuilder.Entity<Game>()
            .HasOne(g => g.PlayerTwo)
            .WithMany() // Ohne Navigationseigenschaft in Player
            .HasForeignKey("PlayerTwoId");

        modelBuilder.Entity<Game>()
            .HasOne(g => g.RobotOne)
            .WithMany() // Ohne Navigationseigenschaft in Robot
            .HasForeignKey("RobotOneId");

        modelBuilder.Entity<Game>()
            .HasOne(g => g.RobotTwo)
            .WithMany() // Ohne Navigationseigenschaft in Robot
            .HasForeignKey("RobotTwoId");

        // Game zu Move (1-zu-m)
        modelBuilder.Entity<Game>()
            .HasOne(g => g.CurrentMove)
            .WithOne(m => m.Game)
            .HasForeignKey<Game>(m => m.CurrentMoveId);

        // Move zu Player (1-zu-1, optional, da ein Move auch von einem Robot stammen kann)
        modelBuilder.Entity<Move>()
            .HasOne(m => m.Player)
            .WithMany()
            .HasForeignKey(m => m.PlayerId)
            .IsRequired(false);

        // Move zu Robot (1-zu-1, optional, da ein Move auch von einem Player stammen kann)
        modelBuilder.Entity<Move>()
            .HasOne(m => m.Robot)
            .WithMany()
            .HasForeignKey(m => m.RobotId)
            .IsRequired(false);
    }

}
