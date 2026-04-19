using Purchase_Orders.Domain.Base;

namespace Purchase_Orders.Domain.Setup
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = default!;
    }
}
