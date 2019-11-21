using Microsoft.EntityFrameworkCore;
using System;
using TeamA.PurchaseOrders.Data.Models;

namespace TeamA.PurchaseOrders.Data
{
    public class PurchaseOrdersDb : DbContext
    {
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseStatus> PurchaseStatus { get; set; }
        public DbSet<PaymentInformation> PaymentInformation { get; set; }

        public PurchaseOrdersDb(DbContextOptions<PurchaseOrdersDb> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PurchaseStatus>(x =>
            {
                x.Property(p => p.ID).IsRequired();
                x.Property(p => p.Name).IsRequired();
            });

            modelBuilder.Entity<PaymentInformation>(x =>
            {
                x.Property(p => p.ID).IsRequired();
                x.Property(p => p.CardholderName).IsRequired();
                x.Property(p => p.CardNumber).IsRequired();
                x.Property(p => p.CVC).IsRequired();
                x.Property(p => p.ExpiryDate).IsRequired();
            });

            modelBuilder.Entity<PurchaseOrder>(x =>
            {
                x.Property(p => p.ID).IsRequired();
                x.Property(p => p.Cost).IsRequired();
                x.Property(p => p.IsDeleted).IsRequired();
                x.HasOne(p => p.PaymentInformation).WithMany().HasForeignKey(p => p.PaymentInformationID).IsRequired();
                x.HasOne(p => p.PurchaseStatus).WithMany().HasForeignKey(p => p.StatusID).IsRequired();
                x.Property(p => p.ProductID).IsRequired();
                x.Property(p => p.ProductName).IsRequired();
                x.Property(p => p.PurchasedBy).IsRequired();
                x.Property(p => p.PurchasedOn).IsRequired();
                x.Property(p => p.Quantity).IsRequired();
                x.Property(p => p.StatusID).IsRequired();
            });
        }

    }
}
