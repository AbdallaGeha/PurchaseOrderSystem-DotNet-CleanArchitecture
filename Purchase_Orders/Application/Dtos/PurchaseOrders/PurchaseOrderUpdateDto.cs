namespace Purchase_Orders.Application.Dtos.PurchaseOrders
{
    public class PurchaseOrderUpdateDto
    {
        public string Date { get; set; } = default!;
        public string Ref { get; set; } = default!;
        public Guid ProjectId { get; set; }
        public Guid SupplierId { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal CurrencyFactor { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal RetentionPercent { get; set; }
        public decimal AdvancePayment { get; set; }
        public string? Remarks { get; set; }
        public List<PurchaseOrderItemUpdateDto> Items { get; set; } = new List<PurchaseOrderItemUpdateDto>();
    }
}
