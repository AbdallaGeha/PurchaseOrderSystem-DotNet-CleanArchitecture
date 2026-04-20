using Purchase_Orders.Domain.Setup;

namespace Purchase_Orders.Application.Contracts.PurchaseOrders.Dtos
{
    /// <summary>
    /// This dto is cross-modules dto used by payment module to get currency info
    /// </summary>
    public class CurrencySnapshotDto
    {
        public Guid CurrencyId { get; set; }
        public decimal CurrencyFactor { get; set; }
    }
}
