using Domain.Enums;
using Domain.Models;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Seeding
{
    public static class ActivitySeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!context.Categories.Any() &&!context.Places.Any() && !context.Activities.Any() && !context.ActivityOccurrences.Any())
            {
                // Kategorier
                var categories = new List<Category>
                {
                    new Category { Id = Guid.NewGuid(), Name = "Inomhusspel" },
                    new Category { Id = Guid.NewGuid(), Name = "Utomhusspel" },
                    new Category { Id = Guid.NewGuid(), Name = "Träning" },
                };

                await context.Categories.AddRangeAsync(categories);

                // Platser
                var place1 = new Place
                {
                    Id = Guid.NewGuid(),
                    Name = "Sporthallen A",
                    Address = "Sporthallsvägen 1, Halmstad",
                    Latitude = 56.6745,
                    Longitude = 12.8572,
                    Environment = EnvironmentType.Indoor,
                    Capacity = 20,
                    IsActive = true
                };

                var place2 = new Place
                {
                    Id = Guid.NewGuid(),
                    Name = "Padelbanor Utomhus",
                    Address = "Padelvägen 5, Falkenberg",
                    Latitude = 56.9050,
                    Longitude = 12.4911,
                    Environment = EnvironmentType.Outdoor,
                    Capacity = 8,
                    IsActive = true
                };

                var place3 = new Place
                {
                    Id = Guid.NewGuid(),
                    Name = "Utegymmet",
                    Address = "Gymparken 12, Varberg",
                    Latitude = 57.1060,
                    Longitude = 12.2500,
                    Environment = EnvironmentType.Outdoor,
                    Capacity = 15,
                    IsActive = true
                };

                await context.Places.AddRangeAsync(place1, place2, place3);

                // Aktiviteter
                var activities = new List<SportActivity>
                {
                    new SportActivity { Id = Guid.NewGuid(), Name = "Klättring", Description="Klättervägg för alla nivåer", Environment=EnvironmentType.Indoor, DefaultDurationMinutes=90, Price=120, IsActive=true, CategoryId = categories.First(c => c.Name == "Inomhusspel").Id },
                    new SportActivity { Id = Guid.NewGuid(), Name = "Pingis", Description="Bordtennis", Environment=EnvironmentType.Indoor, DefaultDurationMinutes=60, Price=50, IsActive=true, CategoryId = categories.First(c => c.Name == "Inomhusspel").Id },
                    new SportActivity { Id = Guid.NewGuid(), Name = "Badminton", Description="Dubbel/Enkel matcher", Environment=EnvironmentType.Indoor, DefaultDurationMinutes=60, Price=80, IsActive=true, CategoryId = categories.First(c => c.Name == "Inomhusspel").Id },
                    new SportActivity { Id = Guid.NewGuid(), Name = "Padel", Description="Padel utomhus", Environment=EnvironmentType.Outdoor, DefaultDurationMinutes=90, Price=150, IsActive=true, CategoryId = categories.First(c => c.Name == "Utomhusspel").Id },
                    new SportActivity { Id = Guid.NewGuid(), Name = "Utegym-pass", Description="Träning i utegymmet", Environment=EnvironmentType.Outdoor, DefaultDurationMinutes=60, Price=0, IsActive=true, CategoryId = categories.First(c => c.Name == "Träning").Id },
                    new SportActivity { Id = Guid.NewGuid(), Name = "Yoga", Description="Inomhus yoga", Environment=EnvironmentType.Indoor, DefaultDurationMinutes=75, Price=100, IsActive=true, CategoryId = categories.First(c => c.Name == "Träning").Id },
                    new SportActivity { Id = Guid.NewGuid(), Name = "Spinning", Description="Cykelpass med instruktör", Environment=EnvironmentType.Indoor, DefaultDurationMinutes=45, Price=90, IsActive=true, CategoryId = categories.First(c => c.Name == "Träning").Id },
                    new SportActivity { Id = Guid.NewGuid(), Name = "Crossfit", Description="Intensivt utepass", Environment=EnvironmentType.Outdoor, DefaultDurationMinutes=60, Price=110, IsActive=true, CategoryId = categories.First(c => c.Name == "Träning").Id },
                };

                await context.Activities.AddRangeAsync(activities);

                // Occurrences
                var occurrences = new List<ActivityOccurrence>();
                var rnd = new Random();
                DateTime startDate = new DateTime(DateTime.UtcNow.Year, 10, 1); // 1 oktober - 30 november
                DateTime endDate = new DateTime(DateTime.UtcNow.Year, 11, 30);

                foreach (var activity in activities)
                {
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        // Skapa 1–2 slumpade tillfällen per dag
                        int slotsPerDay = rnd.Next(1, 3);
                        for (int i = 0; i < slotsPerDay; i++)
                        {
                            int hour = rnd.Next(8, 20); // mellan 08:00–20:00
                            DateTime start = date.AddHours(hour);
                            DateTime end = start.AddMinutes(activity.DefaultDurationMinutes);

                            var placeId = activity.Environment == EnvironmentType.Indoor ? place1.Id :
                                          activity.Name.Contains("Padel") ? place2.Id : place3.Id;

                            occurrences.Add(new ActivityOccurrence
                            {
                                Id = Guid.NewGuid(),
                                ActivityId = activity.Id,
                                PlaceId = placeId,
                                StartUtc = start,
                                EndUtc = end,
                                CapacityOverride = null,
                                PriceOverride = null
                            });
                        }
                    }                 
                }
                await context.ActivityOccurrences.AddRangeAsync(occurrences);

                await context.SaveChangesAsync();
            }
        }
    }
}
