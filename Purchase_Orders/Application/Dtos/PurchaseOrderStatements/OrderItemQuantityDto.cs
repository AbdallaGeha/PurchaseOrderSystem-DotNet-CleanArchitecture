namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class OrderItemQuantityDto
    {
        public Guid PurchaseOrderItemId { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
