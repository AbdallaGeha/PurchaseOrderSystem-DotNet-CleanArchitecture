using Purchase_Orders.Application.IRepositories.Inventory;
using Purchase_Orders.Domain.Inventory;

namespace Purchase_Orders.Data.Repositories.Inventory
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;
        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(InventoryMovement inventoryMovement)
        {
            _context.InventoryMovements.Add(inventoryMovement);
        }
    }
}
