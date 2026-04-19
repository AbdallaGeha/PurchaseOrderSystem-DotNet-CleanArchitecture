using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Purchase_Orders.Application.Dtos.PurchaseOrderStatements
{
    public class PurchaseOrderSnapshotDto
    {
        public int Number { get; set; } = default!;
        public DateOnly Date { get; set; } = default!;
        public string Reference { get; set; } = default!;
        public string Project { get; set; } = default!;
        public string Supplier { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public decimal CurrencyFactor { get; set; }
        public decimal DiscountPercent { get; set; }
        public List<PurchaseOrderItemSnapshotDto> Items { get; set; } = new();
    }
}
