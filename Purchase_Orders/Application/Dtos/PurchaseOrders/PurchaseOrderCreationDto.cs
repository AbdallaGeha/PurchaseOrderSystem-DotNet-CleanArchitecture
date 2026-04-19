using Purchase_Orders.Domain.PurchaseOrders.Enums;
using Purchase_Orders.Domain.PurchaseOrders;
using Purchase_Orders.Domain.Setup;

namespace Purchase_Orders.Application.Dtos.PurchaseOrders
{
    public class PurchaseOrderCreationDto
    {
        public string Date { get; set; } = default!;
        public string Ref { get; set; } = default!;
        public int Number { get; set; }
        public Guid ProjectId { get; set; }
        public Guid SupplierId { get; set; }
        public Guid CurrencyId { get; set; }
        public decimal CurrencyFactor { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal RetentionPercent { get; set; }
        public decimal AdvancePayment { get; set; }
        public string? Remarks { get; set; }
        public List<PurchaseOrderItemCreationDto> Items { get; set; } = new List<PurchaseOrderItemCreationDto>();
    }
}
