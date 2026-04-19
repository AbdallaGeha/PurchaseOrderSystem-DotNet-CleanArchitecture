namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class StatementDto
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public DateOnly Date { get; set; }
        public string Ref { get; set; } = default!;
        public string? Remarks { get; set; }
        public short State { get; set; }
        public int PoNumber { get; set; }
        public string PoDate { get; set; } = default!;
        public string PoReference { get; set; } = default!;
        public string Project { get; set; } = default!;
        public string Supplier { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public decimal CurrencyFactor { get; set; } = default!;
        public List<StatementItemDto> Items { get; set; } = new List<StatementItemDto>();
        public List<StatementViolationDto> Violations { get; set; } = new List<StatementViolationDto>();
    }
}
