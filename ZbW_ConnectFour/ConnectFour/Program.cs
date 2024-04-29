using ConnectFour.Mqtt;
using ConnectFour.Repositories;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace ConnectFour
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Konfiguration laden
			var configuration = builder.Configuration;

			#region Logging

			Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(configuration)
	.CreateLogger();

			builder.Services.AddLogging(loggingBuilder =>
				loggingBuilder.AddSerilog(dispose: true));


			#endregion
						
			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			//builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<GameDbContext>(options =>
					options.UseSqlServer(configuration.GetConnectionString("ConnectFour")).UseLazyLoadingProxies());

			// Füge das Health Check Paket für SQL Server hinzu
			builder.Services.AddHealthChecks()
				.AddSqlServer(configuration.GetConnectionString("ConnectFour"),
					name: "Database", // Name des Health Checks
					failureStatus: HealthStatus.Unhealthy, // Status bei Fehlschlagen
					tags: new[] { "db", "sql", "database" });

			builder.Services.AddScoped<IGenericRepository, GenericRepository>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
			builder.Services.AddScoped<IRobotRepository, RobotRepository>();
			builder.Services.AddScoped<IMoveRepository, MoveRepository>();
			builder.Services.AddScoped<IGameRepository, GameRepository>();

			// Mqtt
			builder.Services.AddSingleton<IMqttService, MqttService>();


			builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
	c.ExampleFilters();
});

			builder.Services.AddSwaggerExamplesFromAssemblyOf<UserRequestExample>();

			var app = builder.Build();
			// Datenbankprüfung und -erstellung
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				var logger = services.GetRequiredService<ILogger<Program>>();
				try
				{
					var context = services.GetRequiredService<GameDbContext>();

					var connected = context.Database.CanConnect();
					if (connected)
					{
						//context.Database.EnsureDeleted();
						logger.LogInformation("Datenbank erfolgreich gelöscht");

					}

					// logger.LogInformation("Es wurde keine Datenbank gefunden. Neue Datenbank wird erstellt...");
					context.Database.EnsureCreated(); // Prüft, ob die DB existiert, und erstellt sie, falls nicht
					logger.LogInformation("Datenbank wurde erfolgreich angelegt!");

				}
				catch (Exception ex)
				{

					logger.LogError(ex, "Ein Fehler ist aufgetreten beim Erstellen der Datenbank.");
				}
			}


			// Konfiguriere Health Checks Route und die Ausgabe
			app.MapHealthChecks("/health", new HealthCheckOptions
			{
				ResponseWriter = async (context, report) =>
				{
					context.Response.ContentType = "application/json";
					var results = report.Entries.Select(e => new
					{
						name = e.Key,
						status = e.Value.Status == HealthStatus.Healthy
					}).ToList();

					var response = new
					{
						checks = results,
						overallStatus = report.Status == HealthStatus.Healthy
					};

					await context.Response.WriteAsJsonAsync(response);
				}
			});

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();

		}
	}
}
