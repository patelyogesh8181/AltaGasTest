using AltaGasTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AltaGasTest.Data
{
    public class AltaGasTestDbContext : DbContext
    {
        public AltaGasTestDbContext(DbContextOptions<AltaGasTestDbContext> options)
            : base(options)
        {
        }
        public DbSet<EventCodeDefinition> EventCodeDefinitions { get; set; }
        public DbSet<CanadianCity> CanadianCities { get; set; }
        public DbSet<EquipmentEvent> EquipmentEvents { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CanadianCity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CityName)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.TimeZone)
                      .HasMaxLength(100)
                      .IsRequired();
            });

            // Seed data – CanadianCity this will land in the initial migration
            modelBuilder.Entity<CanadianCity>().HasData(
                new CanadianCity { Id = 1, CityName = "Vancouver", TimeZone = "Pacific Standard Time" },
                new CanadianCity { Id = 2, CityName = "Victoria", TimeZone = "Pacific Standard Time" },
                new CanadianCity { Id = 3, CityName = "Kelowna", TimeZone = "Pacific Standard Time" },
                new CanadianCity { Id = 4, CityName = "Kamloops", TimeZone = "Pacific Standard Time" },
                new CanadianCity { Id = 5, CityName = "Prince George", TimeZone = "Pacific Standard Time" },
                new CanadianCity { Id = 6, CityName = "Calgary", TimeZone = "Mountain Standard Time" },
                new CanadianCity { Id = 7, CityName = "Edmonton", TimeZone = "Mountain Standard Time" },
                new CanadianCity { Id = 8, CityName = "Lethbridge", TimeZone = "Mountain Standard Time" },
                new CanadianCity { Id = 9, CityName = "Red Deer", TimeZone = "Mountain Standard Time" },
                new CanadianCity { Id = 10, CityName = "Fort McMurray", TimeZone = "Mountain Standard Time" },

                new CanadianCity { Id = 11, CityName = "Regina", TimeZone = "Canada Central Standard Time" },
                new CanadianCity { Id = 12, CityName = "Saskatoon", TimeZone = "Canada Central Standard Time" },
                new CanadianCity { Id = 13, CityName = "Moose Jaw", TimeZone = "Canada Central Standard Time" },
                new CanadianCity { Id = 14, CityName = "Brandon", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 15, CityName = "Winnipeg", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 16, CityName = "Thunder Bay", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 17, CityName = "Sault Ste. Marie", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 18, CityName = "Sudbury", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 19, CityName = "North Bay", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 20, CityName = "Barrie", TimeZone = "Eastern Standard Time" },

                new CanadianCity { Id = 21, CityName = "Toronto", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 22, CityName = "Mississauga", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 23, CityName = "Hamilton", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 24, CityName = "London", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 25, CityName = "Kitchener", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 26, CityName = "Windsor", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 27, CityName = "St. Catharines", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 28, CityName = "Oshawa", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 29, CityName = "Kingston", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 30, CityName = "Ottawa", TimeZone = "Eastern Standard Time" },

                new CanadianCity { Id = 31, CityName = "Gatineau", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 32, CityName = "Montreal", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 33, CityName = "Quebec City", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 34, CityName = "Sherbrooke", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 35, CityName = "Trois-RiviÃ¨res", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 36, CityName = "Saguenay", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 37, CityName = "Rimouski", TimeZone = "Eastern Standard Time" },
                new CanadianCity { Id = 38, CityName = "Edmundston", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 39, CityName = "Fredericton", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 40, CityName = "Moncton", TimeZone = "Atlantic Standard Time" },

                new CanadianCity { Id = 41, CityName = "Saint John", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 42, CityName = "Bathurst", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 43, CityName = "Charlottetown", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 44, CityName = "Summerside", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 45, CityName = "Sydney", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 46, CityName = "Truro", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 47, CityName = "New Glasgow", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 48, CityName = "Dartmouth", TimeZone = "Atlantic Standard Time" },
                new CanadianCity { Id = 49, CityName = "Halifax", TimeZone = "Atlantic Standard Time" }
            );

            modelBuilder.Entity<EventCodeDefinition>(entity =>
            {
                entity.Property(e => e.EventCode)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.EventDescription)
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.LongDescription)
                      .HasMaxLength(200)
                      .IsRequired();
            });

            // Seed data – EventCodeDefinition this will land in the initial migration
            modelBuilder.Entity<EventCodeDefinition>().HasData(
                new EventCodeDefinition { Id = 1, EventCode = "W", EventDescription = "Released", LongDescription = "Railcar equipment is released from origin" },
                new EventCodeDefinition { Id = 2, EventCode = "A", EventDescription = "Arrived", LongDescription = "Railcar equipment arrives at a city on route" },
                new EventCodeDefinition { Id = 3, EventCode = "D", EventDescription = "Departed", LongDescription = "Railcar equipment departs from a city on route" },
                new EventCodeDefinition { Id = 4, EventCode = "Z", EventDescription = "Placed", LongDescription = "Railcar equipment is placed at destination" }
            );
        }
    }
}
