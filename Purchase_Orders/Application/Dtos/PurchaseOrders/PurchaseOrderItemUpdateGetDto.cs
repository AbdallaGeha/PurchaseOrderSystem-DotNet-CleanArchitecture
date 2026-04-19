namespace Purchase_Orders.Application.Dtos.PurchaseOrders
{
    public class PurchaseOrderItemUpdateGetDto
    {
        public Guid Id { get; set; }
        public int LineNo { get; set; }
        public string ItemId { get; set; } = default!;
        public string UnitId { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
    }
}
