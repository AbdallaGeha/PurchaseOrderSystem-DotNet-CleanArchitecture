using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Data;
using Purchase_Orders.Domain.Setup;

namespace PurchaseOrders.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure database is created and migrations applied
            await context.Database.MigrateAsync();

            // Start a transaction
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // =====================
                // Seed Currencies
                // =====================
                if (!await context.Currencies.AnyAsync())
                {
                    var currencies = new List<Currency>
                    {
                        new Currency { Id = Guid.NewGuid(), Name = "Currency A"},
                        new Currency { Id = Guid.NewGuid(), Name = "Currency B"},
                        new Currency { Id = Guid.NewGuid(), Name = "Currency C"}
                    };

                    await context.Currencies.AddRangeAsync(currencies);
                    await context.SaveChangesAsync();
                }

                // =====================
                // Seed Items
                // =====================
                if (!await context.Items.AnyAsync())
                {
                    var items = new List<Item>
                    {
                        new Item
                        {
                            Id = Guid.NewGuid(),
                            Name = "Item 1"                        
                        },
                        new Item
                        {
                            Id = Guid.NewGuid(),
                            Name = "Item 2"
                        }
                    };

                    await context.Items.AddRangeAsync(items);
                    await context.SaveChangesAsync();
                }

                // =====================
                // Seed Units
                // =====================
                if (!await context.Units.AnyAsync())
                {
                    var units = new List<Unit>
                    {
                        new Unit
                        {
                            Id = Guid.NewGuid(),
                            Name = "Unit 1"
                        },
                        new Unit
                        {
                            Id = Guid.NewGuid(),
                            Name = "Unit 2"
                        }
                    };

                    await context.Units.AddRangeAsync(units);
                    await context.SaveChangesAsync();
                }

                // =====================
                // Seed ItemUnits
                // =====================
                if (!await context.ItemUnits.AnyAsync())
                {
                    var items = await context.Items.ToListAsync();
                    var units = await context.Units.ToListAsync();

                    var itemsUnits = new List<ItemUnit>
                    {
                        new ItemUnit
                        {
                            Id = Guid.NewGuid(),
                            ItemId = items[0].Id,
                            UnitId = units[0].Id
                        },
                        new ItemUnit
                        {
                            Id = Guid.NewGuid(),
                            ItemId = items[0].Id,
                            UnitId = units[1].Id
                        },
                        new ItemUnit
                        {
                            Id = Guid.NewGuid(),
                            ItemId = items[1].Id,
                            UnitId = units[0].Id
                        },
                        new ItemUnit
                        {
                            Id = Guid.NewGuid(),
                            ItemId = items[1].Id,
                            UnitId = units[1].Id
                        },
                    };

                    await context.ItemUnits.AddRangeAsync(itemsUnits);
                    await context.SaveChangesAsync();
                }

                // =====================
                // Seed stores
                // =====================
                if (!await context.Stores.AnyAsync())
                {
                    var stores = new List<Store>
                    {
                        new Store
                        {
                            Id = Guid.NewGuid(),
                            Name = "Store 1"
                        },
                        new Store
                        {
                            Id = Guid.NewGuid(),
                            Name = "Store 2"
                        }
                    };

                    await context.Stores.AddRangeAsync(stores);
                    await context.SaveChangesAsync();
                }

                // =====================
                // Seed projects
                // =====================
                if (!await context.Projects.AnyAsync())
                {
                    var stores = await context.Stores.ToListAsync();
                    var projects = new List<Project>
                    {
                        new Project
                        {
                            Id = Guid.NewGuid(),
                            Name = "Project 1",
                            StoreId = stores[0].Id
                        },
                        new Project
                        {
                            Id = Guid.NewGuid(),
                            Name = "Project 2",
                            StoreId = stores[1].Id
                        }
                    };

                    await context.Projects.AddRangeAsync(projects);
                    await context.SaveChangesAsync();
                }

                // =====================
                // Seed Suppliers
                // =====================
                if (!await context.Suppliers.AnyAsync())
                {
                    var suppliers = new List<Supplier>
                    {
                        new Supplier
                        {
                            Id = Guid.NewGuid(),
                            Name = "Supplier 1"
                        },
                        new Supplier
                        {
                            Id = Guid.NewGuid(),
                            Name = "Supplier 2"
                        }                    
                    };

                    await context.Suppliers.AddRangeAsync(suppliers);
                    await context.SaveChangesAsync();
                }

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch
            {
                // Rollback if anything fails
                await transaction.RollbackAsync();
                throw; // Let exception propagate
            }
        }
    }
}
