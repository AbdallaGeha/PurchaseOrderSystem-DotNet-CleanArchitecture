using Purchase_Orders.Domain.Setup;

namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class PurchaseOrderItemSnapshotDto
    {
        public Guid Id { get; set; } 
        public int LineNo { get; set; } 
        public string Item { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public decimal Quantity { get; set; } 
        public decimal UnitPrice { get; set; } 
        public decimal? DiscountPercent { get; set; }
    }
}
