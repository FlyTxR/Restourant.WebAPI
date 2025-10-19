using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingStatus> BookingStatuses { get; set; }
        public DbSet<RestaurantConfiguration> RestaurantConfigurations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura precisione per i campi decimal
            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasPrecision(10, 2);

            // Indice per migliorare performance delle query per categoria
            modelBuilder.Entity<MenuItem>()
                .HasIndex(m => m.CategoryId);

            // Relazione MenuItem -> Category
            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Non permette di cancellare una categoria con menu items

            modelBuilder.Entity<Booking>()
            .HasOne(b => b.BookingStatus)
            .WithMany(bs => bs.Bookings)
            .HasForeignKey(b => b.BookingStatusId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RestaurantConfiguration>()
                .HasIndex(rc => rc.Key)
                .IsUnique();


            modelBuilder.Entity<BookingStatus>().HasData(
                new BookingStatus { Id = 1, Name = "Pending", Description = "In attesa di conferma" },
                new BookingStatus { Id = 2, Name = "Confirmed", Description = "Prenotazione confermata" },
                new BookingStatus { Id = 3, Name = "Seated", Description = "Cliente al tavolo" },
                new BookingStatus { Id = 4, Name = "Completed", Description = "Prenotazione completata" },
                new BookingStatus { Id = 5, Name = "Cancelled", Description = "Prenotazione cancellata" },
                new BookingStatus { Id = 6, Name = "NoShow", Description = "Cliente non presentato" }
            );

            modelBuilder.Entity<RestaurantConfiguration>().HasData(
                new { Id = 1, Key = "MaxCapacityPerSlot", Value = "50", Description = "Capacità massima per fascia oraria" },
                new { Id = 2, Key = "LunchTimes", Value = "12:00,12:30,13:00,13:30,14:00", Description = "Orari pranzo disponibili" },
                new { Id = 3, Key = "DinnerTimes", Value = "19:00,19:30,20:00,20:30,21:00,21:30,22:00", Description = "Orari cena disponibili" },
                new { Id = 4, Key = "RestaurantName", Value = "Roma Antica", Description = "Nome del ristorante" },
                new { Id = 5, Key = "RestaurantPhone", Value = "+39 02 1234567", Description = "Telefono ristorante" },
                new { Id = 6, Key = "RestaurantEmail", Value = "info@romaantica.it", Description = "Email ristorante" }
            );
        }
    }
}
