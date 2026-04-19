namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class NewStatementDto
    {
        public int StatementNumber { get; set; }
        public int PoNumber { get; set; }
        public string PoDate { get; set; } = default!;
        public string PoReference { get; set; } = default!;
        public string Project { get; set; } = default!;
        public string Supplier { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public decimal CurrencyFactor { get; set; } = default!;
        public List<NewStatementItemDto> Items { get; set; } = new List<NewStatementItemDto>();
    }
}
