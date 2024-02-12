using ConnectFour.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

        #region Player

        modelBuilder.Entity<Player>()
           .HasOne(p => p.User)
           .WithOne()
           .HasForeignKey<Player>(p => p.UserId);

        #endregion

        #region Game

        modelBuilder.Entity<Game>()
        .HasMany(e => e.Players)
        .WithMany(e => e.Games);


        modelBuilder.Entity<Game>()
         .HasMany(e => e.Robots)
         .WithMany(e => e.Games);

        modelBuilder.Entity<Game>()
          .HasOne(g => g.WinnerPlayer)
          .WithMany()
          .HasForeignKey(g => g.WinnerPlayerId)
          .IsRequired(false);

        modelBuilder.Entity<Game>()
           .HasOne(g => g.WinnerRobot)
           .WithMany()
           .HasForeignKey(g => g.WinnerRobotId)
           .IsRequired(false);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.CurrentMove)
            .WithOne()
            .HasForeignKey<Game>(m => m.CurrentMoveId)
            .IsRequired(false);

        // Konfigurieren Sie den Speichertyp für TotalPointsPlayerOne und TotalPointsPlayerTwo
        modelBuilder.Entity<Game>()
            .Property(g => g.TotalPointsPlayerOne)
            .HasColumnType("decimal(18, 2)");

        modelBuilder.Entity<Game>()
            .Property(g => g.TotalPointsPlayerTwo)
            .HasColumnType("decimal(18, 2)");

        #endregion


        #region Move

        modelBuilder.Entity<Move>()
        .HasOne(e => e.Game)
        .WithMany()
        .HasForeignKey(e => e.GameId);

        modelBuilder.Entity<Move>()
        .HasOne(m => m.Player)
        .WithMany()
        .HasForeignKey(m => m.PlayerId)
        .IsRequired(false);

        modelBuilder.Entity<Move>()
        .HasOne(m => m.Robot)
        .WithMany()
        .HasForeignKey(m => m.RobotId)
        .IsRequired(false);

        #endregion


    }

}
