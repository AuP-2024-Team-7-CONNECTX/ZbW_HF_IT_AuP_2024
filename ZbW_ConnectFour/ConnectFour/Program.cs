using ConnectFour.Repositories;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<GameDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("LocalNick")).UseLazyLoadingProxies());


            builder.Services.AddScoped<IGenericRepository, GenericRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
            builder.Services.AddScoped<IRobotRepository, RobotRepository>();
            builder.Services.AddScoped<IMoveRepository, MoveRepository>();
            builder.Services.AddScoped<IGameRepository, GameRepository>();



            // Konfigurationswerte aus einer Konfigurationsdatei oder Umgebungsvariablen laden
            var oauthConfig = configuration.GetSection("OAuth2").Get<OAuth2Configuration>();
            builder.Services.AddSingleton(oauthConfig);

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
