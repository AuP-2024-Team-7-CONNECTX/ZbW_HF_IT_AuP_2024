using ConnectFour.Mqtt;
using ConnectFour.Repositories;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace ConnectFour
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			var options = new WebApplicationOptions
			{
				EnvironmentName = environment == "Development" ? Environments.Development : Environments.Production
			};

			var builder = WebApplication.CreateBuilder(options);


			// Configure forwarded headers
			builder.Services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.KnownProxies.Add(IPAddress.Parse("65.109.166.81"));
				options.KnownProxies.Add(IPAddress.Parse("100.87.201.117"));
			});

			if (!(environment == "Development"))
			{
				builder.Configuration
	.SetBasePath("/root/ConnectFour/publish")
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
	.AddEnvironmentVariables();

			}
			else
			{
				// Konfiguration laden
				builder.Configuration
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
	.AddEnvironmentVariables();

			}



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

			builder.Services.AddDbContext<GameDbContext>(options =>
					options.UseSqlServer(configuration.GetConnectionString("ConnectFour")).UseLazyLoadingProxies());

			// F�ge das Health Check Paket f�r SQL Server hinzu
			builder.Services.AddHealthChecks();

			builder.Services.AddScoped<IGenericRepository, GenericRepository>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
			builder.Services.AddScoped<IRobotRepository, RobotRepository>();
			builder.Services.AddScoped<IMoveRepository, MoveRepository>();
			builder.Services.AddScoped<IGameRepository, GameRepository>();

			// Mqtt
			builder.Services.AddSingleton<IMqttService, MqttService>();

			// Mail
			builder.Services.AddSingleton<IEmailSender>(new PostmarkEmailSender("8600a7c6-16a7-4c4f-938e-e144b29f51de", "nick.ponnadu.gmx.ch@zbw-online.ch"));

			builder.Services.AddSingleton<ITokenService, TokenService>();

			builder.Services.AddCors();



			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
				c.ExampleFilters();
			});

			builder.Services.AddSwaggerExamplesFromAssemblyOf<UserRequestExample>();

			var app = builder.Build();

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			// Datenbankpruefung und -erstellung
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
						logger.LogInformation("Datenbank wurde erfolgreich verbunden!");
					}

					context.Database.EnsureCreated(); // Prüft, ob die DB existiert, und erstellt sie, falls nicht
					logger.LogInformation("Datenbank wurde erfolgreich angelegt!");


				}
				catch (Exception ex)
				{
					logger.LogError($"Ein Fehler ist aufgetreten beim Erstellen der Datenbank {ex.Message}");
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

			if (!app.Environment.IsDevelopment())
			{
				app.UseHttpsRedirection();
			}

			app.UseAuthorization();


			app.MapControllers();

			app.UseRouting();


			if (app.Environment.IsDevelopment())
			{
				app.UseCors(builder =>
				{
					builder
						  .AllowAnyOrigin()
						  .AllowAnyMethod()
						  .AllowAnyHeader();


				}
);
			}
			else
			{

				app.UseCors(builder =>
			{
				builder
					  //.WithOrigins("https://connectx.mon3y.ch", "https://localhost:5000")
					  .SetIsOriginAllowedToAllowWildcardSubdomains()
					  .AllowAnyHeader()
					  .AllowCredentials()
					  .WithMethods("GET", "PUT", "POST", "DELETE", "OPTIONS")
					  .SetPreflightMaxAge(TimeSpan.FromSeconds(3600));

			});

			}


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHealthChecks("/health");
			});



			app.Run();

		}
	}
}