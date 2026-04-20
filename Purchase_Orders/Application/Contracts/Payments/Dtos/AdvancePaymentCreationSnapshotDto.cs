namespace Purchase_Orders.Application.Contracts.Payments.Dtos
{
    /// <summary>
    /// This dto is cross-modules dto used by PO module when create advance payment on approval
    /// </summary>
    public class AdvancePaymentCreationSnapshotDto
    {
        public decimal Amount { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal CurrencyFactor { get; set; }
        public Guid PurchaseOrderId { get; set; }
    }
}
