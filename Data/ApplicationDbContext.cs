using Microsoft.EntityFrameworkCore;
using RealtorConnect.Models;

namespace RealtorConnect.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<ApartmentStatus> ApartmentStatuses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Realtor> Realtors { get; set; }
        public DbSet<RealtorGroup> RealtorGroups { get; set; }
        public DbSet<JoinRequest> JoinRequests { get; set; } // Добавляем для запросов на вступление
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Связь Apartment-Realtor
            modelBuilder.Entity<Apartment>()
                .HasOne(a => a.Realtor)
                .WithMany(r => r.Apartments)
                .HasForeignKey(a => a.RealtorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Связь Apartment-Client
            modelBuilder.Entity<Apartment>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Apartments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            // Связь Realtor-RealtorGroup
            modelBuilder.Entity<Realtor>()
                .HasOne(r => r.Group)
                .WithMany(g => g.Realtors)
                .HasForeignKey(r => r.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Связь JoinRequest
            modelBuilder.Entity<JoinRequest>()
                .HasOne(jr => jr.Realtor)
                .WithMany()
                .HasForeignKey(jr => jr.RealtorId);

            modelBuilder.Entity<JoinRequest>()
                .HasOne(jr => jr.Group)
                .WithMany()
                .HasForeignKey(jr => jr.GroupId);

            // Связь Favorite (многие ко многим)
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.ClientId, f.ApartmentId });

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Client)
                .WithMany(c => c.Favorites)
                .HasForeignKey(f => f.ClientId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Apartment)
                .WithMany(a => a.Favorites)
                .HasForeignKey(f => f.ApartmentId);

            // Начальные данные
            modelBuilder.Entity<ApartmentStatus>().HasData(
                new ApartmentStatus { Id = 1, Name = "Доступна" },
                new ApartmentStatus { Id = 2, Name = "Недоступна" }
            );
            modelBuilder.Entity<RealtorGroup>().HasData(
                new RealtorGroup { Id = 1, Name = "Default Group" }
            );
        }
    }
}