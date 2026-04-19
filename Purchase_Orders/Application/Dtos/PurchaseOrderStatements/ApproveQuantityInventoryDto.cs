namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class ApproveQuantityInventoryDto
    {
        public Guid StatementItemId { get; set; }
        public decimal Quantity { get; set; }
        public Guid ItemId { get; set; }
        public Guid UnitId { get; set; }
    }
}
