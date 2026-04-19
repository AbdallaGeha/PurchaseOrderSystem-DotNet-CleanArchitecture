namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class StatementCreationDto
    {
        public DateOnly Date { get; set; }
        public string Ref { get; set; } = default!;
        public int Number { get; set; }
        public string? Remarks { get; set; }
        public Guid PurchaseOrderId { get; set; }
        public List<StatementItemCreationDto> Items { get; set; } = new List<StatementItemCreationDto>();
        public List<StatementViolationCreationDto> Violations { get; set; } = new List<StatementViolationCreationDto>();
    }
}
