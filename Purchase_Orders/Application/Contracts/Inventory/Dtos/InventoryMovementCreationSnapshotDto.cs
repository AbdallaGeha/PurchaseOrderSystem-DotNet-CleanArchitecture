namespace Purchase_Orders.Application.Contracts.Inventory.Dtos
{
    /// <summary>
    /// This dto is cross-modules dto used by PO module when create inventory movement on approve quantity
    /// </summary>
    public class InventoryMovementCreationSnapshotDto
    {
        public Guid StoreId { get; set; }
        public Guid ItemId { get; set; }
        public Guid UnitId { get; set; }
        public decimal Quantity { get; set; }
        public Guid TransactionId { get; set; }
    }
}
