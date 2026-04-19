using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Domain.Base;
using Purchase_Orders.Domain.Inventory;
using Purchase_Orders.Domain.Payments;
using Purchase_Orders.Domain.PurchaseOrders;
using Purchase_Orders.Domain.Setup;

namespace Purchase_Orders.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) :
            base(options)
        { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<ItemUnit> ItemUnits { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<PurchaseOrderStatement> PurchaseOrderStatements { get; set; }
        public DbSet<PurchaseOrderStatementItem> PurchaseOrderStatementItems { get; set; }
        public DbSet<PurchaseOrderViolationDiscount> PurchaseOrderViolationDiscounts { get; set; }
        public DbSet<PurchaseOrderPayment> PurchaseOrderPayments { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure Project entity
            modelBuilder.Entity<Project>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Project>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<Project>()
                .HasOne(x => x.Store)
                .WithOne()
                .HasForeignKey<Project>(x => x.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configure Store entity
            modelBuilder.Entity<Store>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Store>().HasIndex(p => p.Name).IsUnique();

            //Configure Item entity
            modelBuilder.Entity<Item>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Item>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<Item>()
                .HasMany(x => x.Units)
                .WithOne()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configure ItemUnit entity
            modelBuilder.Entity<ItemUnit>()
                .HasIndex(x => x.ItemId);

            //Configure Unit entity
            modelBuilder.Entity<Unit>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Unit>().HasIndex(p => p.Name).IsUnique();

            modelBuilder.Entity<Unit>().HasMany<ItemUnit>()
                .WithOne(x => x.Unit)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configure Currency entity
            modelBuilder.Entity<Currency>().ToTable("Currencies");
            modelBuilder.Entity<Currency>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Currency>().HasIndex(p => p.Name).IsUnique();

            //Configure Supplier entity
            modelBuilder.Entity<Supplier>().Property(p => p.Name).HasMaxLength(50);
            modelBuilder.Entity<Supplier>().HasIndex(p => p.Name).IsUnique();

            //Configure PurchaseOrder entity
            modelBuilder.Entity<PurchaseOrder>().Property(p => p.Ref).HasMaxLength(50);
            modelBuilder.Entity<PurchaseOrder>().HasIndex(p => p.Ref).IsUnique();

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(x => x.Date);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(x => x.ProjectId);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.Supplier)
                .WithMany()
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(x => x.SupplierId);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.CurrencyFactor)
                .HasPrecision(18, 6);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.DiscountPercent)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.RetentionPercent)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.AdvancePayment)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.Remarks)
                .HasMaxLength(255);

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(x => x.State);

            //Configure PurchaseOrderItem entity
            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrderItem>()
                 .HasOne(x => x.Unit)
                 .WithMany()
                 .HasForeignKey(x => x.UnitId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasIndex(x => new { x.ItemId, x.UnitId });

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(p => p.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(p => p.DiscountPercent)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasIndex(x => new { x.PurchaseOrderId, x.LineNo })
                .IsUnique();

            //Configure PurchaseOrderStatement entity
            modelBuilder.Entity<PurchaseOrderStatement>().Property(p => p.Ref).HasMaxLength(50);
            modelBuilder.Entity<PurchaseOrderStatement>().HasIndex(p => p.Ref).IsUnique();

            modelBuilder.Entity<PurchaseOrderStatement>().HasIndex(x => x.Date);
            modelBuilder.Entity<PurchaseOrderStatement>().HasIndex(x => x.Number);
            modelBuilder.Entity<PurchaseOrderStatement>().HasIndex(x => x.State);

            modelBuilder.Entity<PurchaseOrderStatement>().Property(p => p.Remarks).HasMaxLength(255);

            modelBuilder.Entity<PurchaseOrderStatement>()
                .HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.PurchaseOrderStatementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrderStatement>()
                .HasOne<PurchaseOrder>()
                .WithMany()
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrderStatement>()
                .HasIndex(x => x.PurchaseOrderId);

            //Configure PurchaseOrderStatementItem entity
            modelBuilder.Entity<PurchaseOrderStatementItem>()
                .Property(p => p.Quantity)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderStatementItem>()
                .HasOne<PurchaseOrderItem>()
                .WithMany()
                .HasForeignKey(x => x.PurchaseOrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrderStatementItem>()
                .HasIndex(x => x.PurchaseOrderItemId);

            modelBuilder.Entity<PurchaseOrderStatementItem>()
                .HasIndex(x => new { x.PurchaseOrderStatementId, x.LineNo })
                .IsUnique();

            //Configure PurchaseOrderViolationDiscount entity
            modelBuilder.Entity<PurchaseOrderViolationDiscount>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderViolationDiscount>()
                .Property(p => p.Reason)
                .HasMaxLength(255);

            modelBuilder.Entity<PurchaseOrderViolationDiscount>()
                .HasOne<PurchaseOrderStatement>()
                .WithMany(x => x.Violations)
                .HasForeignKey(x => x.PurchaseOrderStatementId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseOrderViolationDiscount>()
                .HasIndex(x => x.PurchaseOrderStatementId);

            //Configure PurchaseOrderPayment entity
            modelBuilder.Entity<PurchaseOrderPayment>()
                .HasIndex(x => new { x.PurchaseOrderId, x.Date });

            modelBuilder.Entity<PurchaseOrderPayment>()
                .HasIndex(x => x.PurchaseOrderStatementId)
                .HasFilter("[PurchaseOrderStatementId] IS NOT NULL");

            modelBuilder.Entity<PurchaseOrderPayment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderPayment>()
                .Property(p => p.AdvanceDeduction)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrderPayment>()
                .Property(p => p.CurrencyFactor)
                .HasPrecision(18, 6);

            modelBuilder.Entity<PurchaseOrderPayment>()
                .HasOne<Currency>()
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrderPayment>()
                .HasOne<PurchaseOrder>()
                .WithMany()
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrderPayment>()
                .HasOne<PurchaseOrderStatement>()
                .WithMany()
                .HasForeignKey(x => x.PurchaseOrderStatementId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configure InventoryMovement entity
            modelBuilder.Entity<InventoryMovement>()
                .HasOne<Store>()
                .WithMany()
                .HasForeignKey(x => x.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryMovement>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryMovement>()
                .HasOne<Unit>()
                .WithMany()
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryMovement>()
                .HasIndex(x => new { x.StoreId, x.Date });

            modelBuilder.Entity<InventoryMovement>()
                .HasIndex(x => new { x.StoreId, x.ItemId, x.UnitId })
                .IncludeProperties(x => new { x.Quantity, x.Date });

            modelBuilder.Entity<InventoryMovement>()
                .HasIndex(x => x.TransactionId);

            modelBuilder.Entity<InventoryMovement>()
                .HasIndex(x => new { x.Date, x.Kind });

            modelBuilder.Entity<InventoryMovement>()
                .Property(p => p.Quantity)
                .HasPrecision(18, 2);
        }

        public override int SaveChanges()
        {
            //Set CreateDate for new entries, LastModifiedDate for new and updated entries
            var now = DateTime.Now;
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        var entity = (BaseEntity)entry.Entity;
                        entity.CreateDate = now;
                        entity.LastModifiedDate = now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        var entity = (BaseEntity)entry.Entity;
                        entity.LastModifiedDate = now;
                    }
                }
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //Set CreateDate for new entries, LastModifiedDate for new and updated entries
            var now = DateTime.Now;
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        var entity = (BaseEntity)entry.Entity;
                        entity.CreateDate = now;
                        entity.LastModifiedDate = now;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        var entity = (BaseEntity)entry.Entity;
                        entity.LastModifiedDate = now;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
