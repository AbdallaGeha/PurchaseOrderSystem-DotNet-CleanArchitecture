namespace Purchase_Orders.Application.Contracts.Payments.Dtos
{
    /// <summary>
    /// This dto is cross-modules dto used by PO module when create retention payment on close
    /// </summary>
    public class RetentionPaymentCreationSnapshotDto
    {
        public decimal Amount { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal CurrencyFactor { get; set; }
        public Guid PurchaseOrderId { get; set; }
    }
}
