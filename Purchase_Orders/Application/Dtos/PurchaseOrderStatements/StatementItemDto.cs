namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class StatementItemDto
    {
        public Guid? Id { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
        public int LineNo { get; set; }
        public string Item { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public decimal QuantityCurrent { get; set; }
        public decimal QuantityPo { get; set; }
        public decimal QuantityAccum { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal AmountPo { get; set; }
        public decimal AmountCurrent { get; set; }
        public decimal AmountAccum { get; set; }
    }
}
