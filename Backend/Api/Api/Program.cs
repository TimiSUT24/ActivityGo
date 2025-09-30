using Domain.Interfaces;                // IUnitOfWork
using Domain.Models;                   // Din User : IdentityUser
using Infrastructure.Persistence;      // Din AppDbContext
using Infrastructure.UnitOfWork;       // Din UnitOfWork
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args); 

            // === 1) Connection string + DbContext (SQL Server) ===
            builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // === 2) Lägg till TimeProvider så Identity inte kraschar vid design-time ===
            builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

            // === 3) Identity (med roller) + EF stores ===
            builder.Services
                .AddIdentityCore<User>(opt =>
                {
                    opt.User.RequireUniqueEmail = true;
                    // Exempel på lättare lösenordspolicy för utveckling:
                    opt.Password.RequiredLength = 6;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireDigit = false;
                })
                .AddRoles<IdentityRole>()                 // om ni använder [Authorize(Roles="Admin")]
                .AddEntityFrameworkStores<AppDbContext>() // Identity-tabeller i samma DB
                .AddSignInManager();

            // === 4) Unit of Work ===
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // === 5) Authorization (lägg till JWT senare) ===
            builder.Services.AddAuthorization();

            // === 6) MVC & Swagger ===
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // === 7) Swagger vid utveckling ===
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Viktigt: AuthN före AuthZ
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}