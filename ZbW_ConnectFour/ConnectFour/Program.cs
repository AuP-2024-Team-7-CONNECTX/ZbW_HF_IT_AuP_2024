
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
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<GameDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("LOCAL")));


            var app = builder.Build();

            // Datenbankprüfung und -erstellung
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<GameDbContext>();
                    context.Database.EnsureCreated(); // Prüft, ob die DB existiert, und erstellt sie, falls nicht
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung, z.B. Logging
                    var logger = services.GetRequiredService<ILogger<Program>>();
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
