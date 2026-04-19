using Purchase_Orders.Domain.Base;
using Purchase_Orders.Domain.Setup;

namespace Purchase_Orders.Domain.PurchaseOrders
{
    public class PurchaseOrderItem : BaseEntity
    {
        public int LineNo { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; } = default!;
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public Guid PurchaseOrderId { get; set; }
    }
}
