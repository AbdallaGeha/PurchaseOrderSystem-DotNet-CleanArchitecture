using Purchase_Orders.Application.Contracts.Inventory.Dtos;

namespace Purchase_Orders.Application.Contracts.Inventory
{
    /// <summary>
    /// This is a contract to be used by other modules to execute inventory commands
    /// </summary>
    public interface IInventoryCommandService
    {
        void CreateInventoryMovement(InventoryMovementCreationSnapshotDto movement);
    }
}
