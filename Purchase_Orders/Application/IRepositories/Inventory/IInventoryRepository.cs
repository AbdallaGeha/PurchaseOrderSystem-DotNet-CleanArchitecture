using Purchase_Orders.Domain.Inventory;

namespace Purchase_Orders.Application.IRepositories.Inventory
{
    public interface IInventoryRepository
    {
        void Add(InventoryMovement inventoryMovement);
    }
}