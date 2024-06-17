using ConnectFour.Interfaces;
using ConnectFour.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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

		var hashPw1 = HashPassword("KI");
		// Initialdaten hinzufügen
		modelBuilder.Entity<User>().HasData(
			new User { Id = Guid.NewGuid().ToString(), Email = "KI_Terminator@ConnectX.ch",Authenticated=true,IsIngame=false,Name= "KI_Terminator@ConnectX.ch", Password= hashPw1 },
			new User { Id = Guid.NewGuid().ToString(), Email = "KI_AgentSmith@ConnectX.ch", Authenticated=true,IsIngame=false,Name= "KI_AgentSmith@ConnectX.ch", Password= hashPw1 }
			
		);


	}

	public static string HashPassword(string password)
	{
		using (SHA256 sha256 = SHA256.Create())
		{
			// Convert the input string to a byte array and compute the hash.
			byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

			// Convert the byte array to a hex string.
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				builder.Append(bytes[i].ToString("x2"));
			}
			return builder.ToString();
		}
	}
}
