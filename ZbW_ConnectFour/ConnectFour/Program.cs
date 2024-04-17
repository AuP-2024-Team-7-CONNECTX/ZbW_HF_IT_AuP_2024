using ConnectFour.Repositories;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
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

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<GameDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("LocalNick")).UseLazyLoadingProxies());

            // Füge das Health Check Paket für SQL Server hinzu
            builder.Services.AddHealthChecks()
                .AddSqlServer(configuration.GetConnectionString("LocalNick"),
                    name: "Database", // Name des Health Checks
                    failureStatus: HealthStatus.Unhealthy, // Status bei Fehlschlagen
                    tags: new[] { "db", "sql", "database" });

            builder.Services.AddScoped<IGenericRepository, GenericRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
            builder.Services.AddScoped<IRobotRepository, RobotRepository>();
            builder.Services.AddScoped<IMoveRepository, MoveRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();


                        // Weitere benötigte Services registrieren, z.B. für die E-Mail-Verifizierung
            builder.Services.AddTransient<EmailVerificationService>();

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
                        logger.LogInformation("Datenbank erfolgreich verbunden");

                    }
                    else
                    {
                        logger.LogInformation("Es wurde keine Datenbank gefunden. Neue Datenbank wird erstellt...");
                        context.Database.EnsureCreated(); // Prüft, ob die DB existiert, und erstellt sie, falls nicht
                        logger.LogInformation("Datenbank wurde erfolgreich angelegt!");
                    }

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

            //string brokerAddress = "broker.hivemq.com";
            //var mqttClientService = new MqttClientService(brokerAddress);

            //mqttClientService.Connect("testClientId");

            //// Abonniere Topics
            //string[] topics = { "test/topic" };
            //byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            //mqttClientService.Subscribe(topics, qosLevels);

            //// Sende eine Nachricht
            //mqttClientService.Publish("test/topic", "Hello MQTT from M2MqttDotnetCore!");

            //Console.WriteLine("Press any key to exit...");
            //Console.ReadKey();

            //mqttClientService.Disconnect();
        }
    }
}
