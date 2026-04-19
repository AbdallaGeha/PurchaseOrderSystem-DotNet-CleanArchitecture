using Purchase_Orders.Domain.Base;

namespace Purchase_Orders.Domain.PurchaseOrders
{
    public class PurchaseOrderViolationDiscount : BaseEntity
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = default!;
        public Guid PurchaseOrderStatementId { get; set; }
    }
}
