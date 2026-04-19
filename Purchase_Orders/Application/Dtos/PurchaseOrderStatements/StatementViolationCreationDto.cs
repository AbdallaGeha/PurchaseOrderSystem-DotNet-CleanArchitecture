namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class StatementViolationCreationDto
    {
        public decimal Amount { get; set; }
        public string Reason { get; set; } = default!;
    }
}
