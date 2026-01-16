using ClothsStoreSys.Models;
using Microsoft.EntityFrameworkCore;

namespace ClothsStoreSys.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnItem> ReturnItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision to avoid SQL truncation warnings
            modelBuilder.Entity<Item>().Property(i => i.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Return>().Property(r => r.Total).HasPrecision(18, 2);
            modelBuilder.Entity<ReturnItem>().Property(ri => ri.Amount).HasPrecision(18, 2);

            // A Return references an Invoice. Prevent cascade delete from Invoice -> Return to avoid multiple cascade paths
            modelBuilder.Entity<Return>()
                .HasOne(r => r.Invoice)
                .WithMany() // Invoice does not have a collection of Returns
                .HasForeignKey(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // A ReturnItem references an InvoiceItem. Prevent cascade delete from InvoiceItem -> ReturnItem to avoid multiple cascade paths
            modelBuilder.Entity<ReturnItem>()
                .HasOne(ri => ri.InvoiceItem)
                .WithMany() // InvoiceItem does not have a collection of ReturnItems
                .HasForeignKey(ri => ri.InvoiceItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure Return -> ReturnItems uses cascade delete (deleting a Return removes its items)
            modelBuilder.Entity<ReturnItem>()
                .HasOne(ri => ri.Return)
                .WithMany(r => r.Items)
                .HasForeignKey(ri => ri.ReturnId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}