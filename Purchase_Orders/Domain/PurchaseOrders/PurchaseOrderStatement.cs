using Purchase_Orders.Domain.Base;
using Purchase_Orders.Domain.PurchaseOrders.Enums;
using System.Security.Principal;

namespace Purchase_Orders.Domain.PurchaseOrders
{
    public class PurchaseOrderStatement : BaseEntity
    {
        public DateOnly Date { get; set; }
        public string Ref { get; set; } = default!;
        public int Number { get; set; }
        public PurchaseOrderStatementState State { get; set; }
        public string? Remarks { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public List<PurchaseOrderStatementItem> Items { get; set; } = new List<PurchaseOrderStatementItem>();
        public List<PurchaseOrderViolationDiscount> Violations { get; set; } = new List<PurchaseOrderViolationDiscount>();
    }
}
