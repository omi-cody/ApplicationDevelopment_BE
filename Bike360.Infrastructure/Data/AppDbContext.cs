using Bike360.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Bike360.Infrastructure.Data
{
    
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        // Customer DbSets 
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<SaleInvoice> SaleInvoices => Set<SaleInvoice>();
        public DbSet<SaleInvoiceItem> SaleInvoiceItems => Set<SaleInvoiceItem>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<PartRequest> PartRequests => Set<PartRequest>();
        public DbSet<ServiceReview> ServiceReviews => Set<ServiceReview>();

       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Your existing Identity table mappings
            modelBuilder.Entity<IdentityRole>().ToTable("userRoles");
            modelBuilder.Entity<ApplicationUser>().ToTable("users");

            // Customer 
            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(c => c.Id);
                e.HasIndex(c => c.Email).IsUnique();
                e.HasIndex(c => c.ApplicationUserId).IsUnique();
                e.Property(c => c.FullName).IsRequired().HasMaxLength(100);
                e.Property(c => c.Email).IsRequired().HasMaxLength(200);
                e.Property(c => c.PhoneNumber).HasMaxLength(20);
                e.Property(c => c.Address).HasMaxLength(300);
                e.Property(c => c.OutstandingCredit).HasColumnType("numeric(18,2)");
            });

            // Vehicle 
            modelBuilder.Entity<Vehicle>(e =>
            {
                e.HasKey(v => v.Id);
                e.HasIndex(v => v.VehicleNumber).IsUnique();
                e.Property(v => v.VehicleNumber).IsRequired().HasMaxLength(20);
                e.Property(v => v.Make).IsRequired().HasMaxLength(50);
                e.Property(v => v.Model).IsRequired().HasMaxLength(50);
                e.Property(v => v.VehicleType).HasMaxLength(30);

                e.HasOne(v => v.Customer)
                 .WithMany(c => c.Vehicles)
                 .HasForeignKey(v => v.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── SaleInvoice ───────────────────────────────────────────────────
            modelBuilder.Entity<SaleInvoice>(e =>
            {
                e.HasKey(i => i.Id);
                e.HasIndex(i => i.InvoiceNumber).IsUnique();
                e.Property(i => i.SubTotal).HasColumnType("numeric(18,2)");
                e.Property(i => i.DiscountAmount).HasColumnType("numeric(18,2)");
                e.Property(i => i.TotalAmount).HasColumnType("numeric(18,2)");

                e.HasOne(i => i.Customer)
                 .WithMany(c => c.SaleInvoices)
                 .HasForeignKey(i => i.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(i => i.Vehicle)
                 .WithMany(v => v.SaleInvoices)
                 .HasForeignKey(i => i.VehicleId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            //  SaleInvoiceItem 
            modelBuilder.Entity<SaleInvoiceItem>(e =>
            {
                e.HasKey(i => i.Id);
                e.Property(i => i.PartName).IsRequired().HasMaxLength(150);
                e.Property(i => i.UnitPrice).HasColumnType("numeric(18,2)");
                e.Ignore(i => i.LineTotal); 

                e.HasOne(i => i.SaleInvoice)
                 .WithMany(inv => inv.Items)
                 .HasForeignKey(i => i.SaleInvoiceId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            //  Appointment 
            modelBuilder.Entity<Appointment>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.ServiceType).IsRequired().HasMaxLength(100);
                e.Property(a => a.Notes).HasMaxLength(500);

                e.HasOne(a => a.Customer)
                 .WithMany(c => c.Appointments)
                 .HasForeignKey(a => a.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(a => a.Vehicle)
                 .WithMany()
                 .HasForeignKey(a => a.VehicleId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //  PartRequest 
            modelBuilder.Entity<PartRequest>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.PartName).IsRequired().HasMaxLength(200);
                e.Property(r => r.Description).HasMaxLength(500);

                e.HasOne(r => r.Customer)
                 .WithMany(c => c.PartRequests)
                 .HasForeignKey(r => r.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ServiceReview 
            modelBuilder.Entity<ServiceReview>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Title).IsRequired().HasMaxLength(150);
                e.Property(r => r.Comment).IsRequired().HasMaxLength(1000);

                e.HasOne(r => r.Customer)
                 .WithMany(c => c.ServiceReviews)
                 .HasForeignKey(r => r.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}