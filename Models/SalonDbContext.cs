using Microsoft.EntityFrameworkCore;

namespace KuaförRandevuSistemi.Models
{
    public class SalonDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

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

            // TPH Discriminator column setup
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<User>("User")
                .HasValue<Staff>("Staff");

            // One-to-one or one-to-none relationship between Staff and Specialty
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.Specialty)
                .WithMany()
                .HasForeignKey(s => s.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many relationship between Staff and Services
            modelBuilder.Entity<Staff>()
                .HasMany(s => s.Services)
                .WithMany()
                .UsingEntity(j => j.ToTable("StaffServices"));
        }



    }
}
