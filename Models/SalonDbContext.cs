using Microsoft.EntityFrameworkCore;

namespace KuaförRandevuSistemi.Models
{
    public class SalonDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Replace with your PostgreSQL connection string
                optionsBuilder.UseNpgsql("Host=localhost;Database=HairSalonDB;Username=postgres;Password=uk332003");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationships or seed data here if needed
            modelBuilder.Entity<Staff>()
                .HasMany(s => s.Services)
                .WithMany(s => s.Staff)
                .UsingEntity(j => j.ToTable("StaffServices"));
        }
    }
}
