using Purchase_Orders.Domain.Base;
using System.Reflection.PortableExecutable;

namespace Purchase_Orders.Domain.Setup
{
    public class ItemUnit : BaseEntity
    {
        public Guid ItemId { get; set; }
        public Guid UnitId { get; set; }
        public Unit Unit { get; set; } = default!;
    }
}
