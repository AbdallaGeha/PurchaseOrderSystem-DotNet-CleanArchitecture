using Purchase_Orders.Domain.Base;

namespace Purchase_Orders.Domain.Setup
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = default!;
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;
    }
}
