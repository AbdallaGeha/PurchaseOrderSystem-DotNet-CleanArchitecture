using Purchase_Orders.Domain.Base;

namespace Purchase_Orders.Domain.PurchaseOrders
{
    public class PurchaseOrderStatementItem : BaseEntity
    {
        public int LineNo { get; set; }
        public decimal Quantity { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
        public Guid PurchaseOrderStatementId { get; set; }
    }
}
