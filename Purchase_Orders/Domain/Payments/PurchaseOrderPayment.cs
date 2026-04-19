using Purchase_Orders.Domain.Base;
using Purchase_Orders.Domain.Payments.Enums;

namespace Purchase_Orders.Domain.Payments
{
    public class PurchaseOrderPayment : BaseEntity
    {
        public DateOnly Date { get; set; }
        public PaymentType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal AdvanceDeduction { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal CurrencyFactor { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public Guid? PurchaseOrderStatementId { get; set; }
    }
}
