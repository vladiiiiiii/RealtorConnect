using Microsoft.EntityFrameworkCore;
using RealtorConnect.Models;

namespace RealtorConnect.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<ApartmentStatus> ApartmentStatuses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Realtor> Realtors { get; set; }
        public DbSet<RealtorGroup> RealtorGroups { get; set; }
        public DbSet<GroupRealtor> GroupRealtors { get; set; }
        public DbSet<GroupClient> GroupClients { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ViewHistory> ViewHistories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи ChatMessage -> SenderClient
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.SenderClient)
                .WithMany(c => c.SentMessages)
                .HasForeignKey(cm => cm.SenderClientId)
                .HasPrincipalKey(c => c.Id) // Явно указываем первичный ключ
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи ChatMessage -> SenderRealtor
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.SenderRealtor)
                .WithMany(r => r.SentMessages)
                .HasForeignKey(cm => cm.SenderRealtorId)
                .HasPrincipalKey(r => r.Id) // Явно указываем первичный ключ
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи ChatMessage -> ReceiverClient
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ReceiverClient)
                .WithMany(c => c.ReceivedMessages)
                .HasForeignKey(cm => cm.ReceiverClientId)
                .IsRequired(false) // Разрешаем null
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи ChatMessage -> ReceiverRealtor
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.ReceiverRealtor)
                .WithMany(r => r.ReceivedMessages)
                .IsRequired(false) // Разрешаем null
                .HasForeignKey(cm => cm.ReceiverRealtorId)
                .OnDelete(DeleteBehavior.Restrict);
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

            // Связь Favorite (многие ко многим)
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.ClientId, f.ApartmentId });

            // Связь ViewHistory (многие ко многим)
            modelBuilder.Entity<ViewHistory>()
                .HasKey(vh => new { vh.ClientId, vh.ApartmentId });

            // Связь Client-GroupClient (один-к-одному)
            modelBuilder.Entity<Client>()
                .HasOne(c => c.GroupClient)
                .WithOne(gc => gc.Client)
                .HasForeignKey<Client>(c => c.GroupClientId)
                .OnDelete(DeleteBehavior.SetNull);

            // Связь GroupClient-RealtorGroup (многие-к-одному)
            modelBuilder.Entity<GroupClient>()
                .HasOne(gc => gc.RealtorGroup)
                .WithMany(g => g.GroupClients)
                .HasForeignKey(gc => gc.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь Realtor-GroupRealtor (один-к-одному)
            modelBuilder.Entity<Realtor>()
                .HasOne(r => r.GroupRealtor)
                .WithOne(gr => gr.Realtor)
                .HasForeignKey<Realtor>(r => r.GroupRealtorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь GroupRealtor-RealtorGroup (многие-к-одному)
            modelBuilder.Entity<GroupRealtor>()
                .HasOne(gr => gr.RealtorGroup)
                .WithMany(g => g.GroupRealtors)
                .HasForeignKey(gr => gr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

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