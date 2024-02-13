
using ConnectFour.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
                    options.UseSqlServer(configuration.GetConnectionString("LocalNick")));


            builder.Services.AddScoped<IGenericRepository, GenericRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();



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
