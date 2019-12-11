using Microsoft.EntityFrameworkCore;
using TeamA.PurchaseOrders.Models.Dtos;

namespace TeamA.PurchaseOrders.Data
{
    public class PurchaseOrdersDb : DbContext
    {
        public DbSet<PurchaseOrderDto> PurchaseOrders { get; set; }
        public DbSet<PurchaseStatusDto> PurchaseStatus { get; set; }
        public DbSet<PaymentInformationDto> PaymentInformation { get; set; }

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

            modelBuilder.Entity<PurchaseStatusDto>(x =>
            {
                x.Property(p => p.Id).IsRequired();
                x.Property(p => p.Name).IsRequired();
            });

            modelBuilder.Entity<PaymentInformationDto>(x =>
            {
                x.Property(p => p.ID).IsRequired();
                x.Property(p => p.CardName).IsRequired();
                x.Property(p => p.CardNumber).IsRequired();
                x.Property(p => p.CardCVC).IsRequired();
                x.Property(p => p.CardExpiry).IsRequired();
            });

            modelBuilder.Entity<PurchaseOrderDto>(x =>
            {
                x.Property(p => p.ID).IsRequired();
                x.Property(p => p.ProductPrice).IsRequired();
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
