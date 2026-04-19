namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class StatementItemCreationDto
    {
        public int LineNo { get; set; }
        public decimal Quantity { get; set; }
        public Guid PurchaseOrderItemId { get; set; }
    }
}
