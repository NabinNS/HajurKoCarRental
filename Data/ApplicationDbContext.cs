using HajurKoCarRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HajurKoCarRental.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Damage> Damages { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<RentalRequest> RentalRequests { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            // 1-to-many relationship between ApplicationUser and RentalRequest
            modelBuilder.Entity<RentalRequest>()
                .HasOne(r => r.User)
                .WithMany(u => u.RentalRequests)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-to-1 relationship between RentalRequest and ApplicationUser for AuthorizedBy
            modelBuilder.Entity<RentalRequest>()
                .HasOne(r => r.AuthorizedByUser)
                .WithMany()
                .HasForeignKey(r => r.AuthorizedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-to-many relationship between Car and Offer
            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Car)
                .WithMany(c => c.Offers)
                .HasForeignKey(o => o.CarID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-many relationship between Car and RentalRequest
            modelBuilder.Entity<RentalRequest>()
                .HasOne(r => r.Car)
                .WithMany(c => c.RentalRequests)
                .HasForeignKey(r => r.CarID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-many relationship between Car and Damage
            modelBuilder.Entity<Damage>()
                .HasOne(d => d.Car)
                .WithMany(c => c.Damages)
                .HasForeignKey(d => d.CarID)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-many relationship between AppUser and Damage
            modelBuilder.Entity<Damage>()
                .HasOne(d => d.User)
                .WithMany(u => u.Damages)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Damage>()
                .HasOne(d => d.RentalRequest)
                .WithMany(r => r.Damages)
                .HasForeignKey(d => d.RentalID)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
