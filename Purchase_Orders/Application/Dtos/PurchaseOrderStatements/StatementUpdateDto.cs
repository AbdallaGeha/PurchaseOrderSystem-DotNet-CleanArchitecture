namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class StatementUpdateDto
    {
        public DateOnly Date { get; set; }
        public string Ref { get; set; } = default!;
        public string? Remarks { get; set; }
        public List<StatementItemUpdateDto> Items { get; set; } = new List<StatementItemUpdateDto>();
        public List<StatementViolationUpdateDto> Violations { get; set; } = new List<StatementViolationUpdateDto>();
    }
}
