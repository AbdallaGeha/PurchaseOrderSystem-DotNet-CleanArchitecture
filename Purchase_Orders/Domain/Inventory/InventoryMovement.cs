using Purchase_Orders.Domain.Base;
using Purchase_Orders.Domain.Inventory.Enums;

namespace Purchase_Orders.Domain.Inventory
{
    public class InventoryMovement : BaseEntity
    {
        public DateOnly Date { get; set; }
        public InventoryMovementKind Kind { get; set; }
        public Guid StoreId { get; set; }
        public Guid ItemId { get; set; }
        public Guid UnitId { get; set; }
        public decimal Quantity { get; set; }
        public Guid TransactionId { get; set; }
    }
}
