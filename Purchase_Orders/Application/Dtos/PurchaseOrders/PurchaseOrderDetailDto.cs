using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Purchase_Orders.Application.Dtos.PurchaseOrders
{
    public class PurchaseOrderDetailDto
    {
        public int Number { get; set; } = default!;
        public DateOnly Date { get; set; } = default!;
        public string Reference { get; set; } = default!;
        public string Project { get; set; } = default!;
        public string Supplier { get; set; } = default!;
        public string Currency { get; set; } = default!;
        public decimal CurrencyFactor { get; set; }
        public decimal DiscountPercent { get; set; }
        public List<PurchaseOrderItemDetailDto> Items { get; set; } = new();
    }
}
