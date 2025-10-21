using Api.Middleware;
using Application.Activity.Interface;
using Application.Activity.Mapper;
using Application.Activity.Service;
using Application.ActivityOccurrence.Interface;
using Application.ActivityOccurrence.Mapper;
using Application.ActivityOccurrence.Service;
using Application.Auth.Interface;
using Application.Auth.Mapper;
using Application.Auth.Service;
using Application.Booking.Interface;
using Application.Booking.Mapper;
using Application.Booking.Service;
using Application.Place.Interface;
using Application.Place.Mapper;
using Application.Place.Service;
using Application.Services.Interfaces;
using Application.Services;
using Application.Statistics.Interface;
using Application.Statistics.Mapper;
using Application.Statistics.Service;
using Application.Weather.Interface;
using Application.Weather.Interfaces;
using Application.Weather.Mapper;
using Application.Weather.Service;
using Domain.Interfaces;                // IUnitOfWork
using Domain.Models;                   // Din User : IdentityUser
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Auth;             // <-- TokenService s
using Infrastructure.Data.Seeding;
using Infrastructure.Persistence;      // Din AppDbContext
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;       // Din UnitOfWork
using Infrastructure.Weather;

// === JWT usings ===
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
// === Swagger usings ===
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.BackgroundJobs;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //AddScoped Services
            builder.Services.AddScoped<IPlaceService, PlaceService>();
            builder.Services.AddScoped<IActivityOccurrenceService, ActivityOccurrenceService>();
            builder.Services.AddScoped<IActivityService, ActivityService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IStatisticsService, StatisticsService>();
            builder.Services.AddScoped<IWeatherService, WeatherService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            //Fake Weather client for testing and development without API key
            builder.Services.AddSingleton<IWeatherClient, FakeWeatherClient>();
            //AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                // optional: additional configuration here
            },
            typeof(ActivityProfile), 
            typeof(ActivityOccurrenceProfile), 
            typeof(AuthProfile),
               typeof(BookingProfile), 
               typeof(PlaceProfile), 
               typeof(StatisticsProfile), 
               typeof(WeatherProfile));

            // ===  Connection string + DbContext (SQL Server) ===
            builder.Services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // === Weather cache + options ===
            builder.Services.AddMemoryCache();

            builder.Services.Configure<OpenWeatherOptions>(
                builder.Configuration.GetSection("OpenWeather"));

            // Välj klient: Fake i dev utan nyckel, annars riktig HTTP-klient
            var wxKey = builder.Configuration["OpenWeather:ApiKey"];
            if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(wxKey))
            {
                // Utv utan nyckel = fake
                builder.Services.AddSingleton<IWeatherClient, FakeWeatherClient>();
            }
            else
            {
                builder.Services.AddHttpClient<IWeatherClient, OpenWeatherClient>(c =>
                {
                    c.Timeout = TimeSpan.FromSeconds(8);
                });
            }



            // ===  Lägg till TimeProvider så Identity inte kraschar vid design-time ===
            builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

            // ===  Identity (med roller) + EF stores ===
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

            // ===  Unit of Work + Repositorys ===
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IPlaceRepository, PlaceRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IActivityOccurrenceRepository, ActivityOccurrenceRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IActivityPlaceRepository, ActivityPlaceRepository>();

            // === Hosted Service ===
            builder.Services.AddHostedService<BookingStatusRefresher>();


            // ===  Validation ===   // Glöm inte att man bara behöver registrera detta en gång då den läser av alla Validators i Application.
            builder.Services.AddValidatorsFromAssembly(typeof(Application.Activity.Validator.ActivityCreateValidator).Assembly);
            builder.Services.AddFluentValidationAutoValidation();
            // ===  JWT Authentication ===
            var jwt = builder.Configuration.GetSection("Jwt");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidAudience = jwt["Audience"],
                        IssuerSigningKey = signingKey,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // ===  Authorization (JWT + roller) ===
            builder.Services.AddAuthorization();

            // ===  MVC & Swagger ===
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ActiviGo API", Version = "v1" });

                // === JWT i Swagger (Authorize-knappen) ===
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Skriv: Bearer {ditt_jwt}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            //Cors configuration
            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                        policy.WithOrigins(allowedOrigins!)
                          .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            app.UseCors("AllowFrontend");

            //Seeding roles and users 
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await UserSeed.SeedUsersAndRolesAsync(userManager, roleManager);
                await ActivitySeed.SeedAsync(services.GetRequiredService<AppDbContext>());

            }

            // ===  Swagger vid utveckling ===
            if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

            app.UseHttpsRedirection();
            app.UseMiddleware<GlobalExceptionMiddleware>();
            // Viktigt: AuthN före AuthZ
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}