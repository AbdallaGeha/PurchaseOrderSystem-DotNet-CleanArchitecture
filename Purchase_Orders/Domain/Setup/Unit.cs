using Purchase_Orders.Domain.Base;

namespace Purchase_Orders.Domain.Setup
{
    public class Unit : BaseEntity
    {
        public string Name { get; set; } = default!;
    }
}
