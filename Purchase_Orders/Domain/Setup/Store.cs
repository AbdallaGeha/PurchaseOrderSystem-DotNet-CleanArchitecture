using Purchase_Orders.Domain.Base;

namespace Purchase_Orders.Domain.Setup
{
    public class Store : BaseEntity
    {
        public string Name { get; set; } = default!;
    }
}
