using Purchase_Orders.Application.Contracts.Inventory;
using Purchase_Orders.Application.Contracts.Inventory.Dtos;
using Purchase_Orders.Application.IRepositories.Inventory;
using Purchase_Orders.Domain.Inventory.Enums;
using Purchase_Orders.Domain.Inventory;
using Purchase_Orders.Domain.Setup;

namespace Purchase_Orders.Application.ModuleAPIs.Inventory
{
    public class InventoryCommandService : IInventoryCommandService
    {
        private readonly IInventoryRepository _inventoryRepository;
        public InventoryCommandService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public void CreateInventoryMovement(InventoryMovementCreationSnapshotDto movement)
        {
            _inventoryRepository.Add(new InventoryMovement
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Kind = InventoryMovementKind.PoReceipt,
                StoreId = movement.StoreId,
                ItemId = movement.ItemId,
                UnitId = movement.UnitId,
                Quantity = movement.Quantity,
                TransactionId = movement.TransactionId
            });
        }
    }
}
