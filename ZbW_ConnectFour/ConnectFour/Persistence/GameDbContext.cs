using ConnectFour.Interfaces;
using ConnectFour.Models;
using Microsoft.EntityFrameworkCore;

public class GameDbContext : DbContext
{
	public DbSet<Game> Games { get; set; }
	public DbSet<Move> Moves { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Robot> Robots { get; set; }

	public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

	public override int SaveChanges(bool acceptAllChangesOnSuccess)
	{
		SetInsertedOnForAddedEntities();
		return base.SaveChanges(acceptAllChangesOnSuccess);
	}

	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
	{
		SetInsertedOnForAddedEntities();
		return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	private void SetInsertedOnForAddedEntities()
	{
		var addedEntities = ChangeTracker.Entries<Entity>()
			.Where(entry => entry.State == EntityState.Added)
			.Select(entry => entry.Entity);

		foreach (var entity in addedEntities)
		{
			entity.InsertedOn = DateTime.UtcNow;
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{

		base.OnModelCreating(modelBuilder);

		#region Game

		modelBuilder.Entity<Game>()
		 .HasMany(e => e.Robots);

		modelBuilder.Entity<Game>()
	   .HasMany(e => e.Users);

		modelBuilder.Entity<Game>()
		  .HasOne(g => g.WinnerUser)
		  .WithMany()
		  .HasForeignKey(g => g.WinnerUserId)
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
			.Property(g => g.TotalPointsUserOne)
			.HasColumnType("decimal(18, 2)");

		modelBuilder.Entity<Game>()
			.Property(g => g.TotalPointsUserTwo)
			.HasColumnType("decimal(18, 2)");

		#endregion


		#region Move

		modelBuilder.Entity<Move>()
		.HasOne(e => e.Game)
		.WithMany()
		.HasForeignKey(e => e.GameId);

		modelBuilder.Entity<Move>()
		.HasOne(m => m.User)
		.WithMany()
		.HasForeignKey(m => m.PlayerId)
		.IsRequired(false);

		modelBuilder.Entity<Move>()
		.HasOne(m => m.Robot)
		.WithMany()
		.HasForeignKey(m => m.RobotId)
		.IsRequired(false);

		#endregion

		// Initialdaten hinzufügen
		modelBuilder.Entity<User>().HasData(
			new User { Id = Guid.NewGuid().ToString(), Email = "KI (Terminator)",Authenticated=true,IsIngame=false,Name="KI (Terminator)",Password="KI1" },
			new User { Id = Guid.NewGuid().ToString(), Email = "KI (Agent Smith)",Authenticated=true,IsIngame=false,Name="KI (Agent Smith)",Password="KI2" }
			
		);


	}

}
