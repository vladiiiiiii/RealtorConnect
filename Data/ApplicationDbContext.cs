using Microsoft.EntityFrameworkCore;
using RealtorConnect.Models;

namespace RealtorConnect.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<ApartmentStatus> ApartmentStatuses { get; set; } // Только если статусы не enum
        public DbSet<Realtor> Realtors { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<GroupClient> GroupClients { get; set; }
        public DbSet<GroupRealtor> GroupRealtors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальный Email
            modelBuilder.Entity<Realtor>()
                .HasIndex(r => r.Email)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Связь многие ко многим "Клиент - Группа клиентов"
            modelBuilder.Entity<GroupClient>()
                .HasKey(gc => new { gc.GroupId, gc.ClientId });

            // Связь многие ко многим "Риэлтор - Группа риэлторов"
            modelBuilder.Entity<GroupRealtor>()
                .HasKey(gr => new { gr.GroupId, gr.RealtorId });

            // Связь многие ко многим "Клиент - Избранные квартиры"
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.ClientId, f.ApartmentId });

            // Начальные данные для статусов квартир
            modelBuilder.Entity<ApartmentStatus>().HasData(
                new ApartmentStatus { Id = 1, Name = "Available" },
                new ApartmentStatus { Id = 2, Name = "Sold" },
                new ApartmentStatus { Id = 3, Name = "Rented" }
            );
        }
    }
}
