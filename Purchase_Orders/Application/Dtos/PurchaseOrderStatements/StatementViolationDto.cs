namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class StatementViolationDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = default!;
    }
}
