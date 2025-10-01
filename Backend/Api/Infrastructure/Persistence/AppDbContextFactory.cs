using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence
{
    /// <summary>
    /// Används av EF-verktygen vid design-time (migrations/updates) för att skapa AppDbContext
    /// utan att behöva köra hela Program.cs/DI.
    /// </summary>
    public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Försök läsa connection string från env var först (säkrast)
            var envCs = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            // Hitta appsettings.json (vi antar lösningsstruktur med Api/ och Infrastructure/)
            var basePath = Directory.GetCurrentDirectory();
            var apiPath = basePath;

            // Om vi står i Infrastructure-katalogen, prova peka på Api-katalogen för appsettings
            if (Path.GetFileName(basePath).Equals("Infrastructure", StringComparison.OrdinalIgnoreCase))
            {
                var parent = Directory.GetParent(basePath)?.FullName ?? basePath;
                var maybeApi = Path.Combine(parent, "Api");
                if (Directory.Exists(maybeApi))
                    apiPath = maybeApi;
            }

            var cfg = new ConfigurationBuilder()
                .SetBasePath(apiPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var cs =
                envCs ??
                cfg.GetConnectionString("DefaultConnection") ??
                // Fallback för dev om inget hittas
                "Server=localhost,1433;Database=ActiviGoDB;User ID=sa;Password=MyStrongPass123;TrustServerCertificate=True;";

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(cs)
                .Options;

            return new AppDbContext(options);
        }
    }
}