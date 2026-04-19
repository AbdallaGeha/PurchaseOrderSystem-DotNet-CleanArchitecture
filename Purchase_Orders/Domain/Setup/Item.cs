using Purchase_Orders.Domain.Base;
using System.Reflection.PortableExecutable;

namespace Purchase_Orders.Domain.Setup
{
    public class Item : BaseEntity
    {
        public string Name { get; set; } = default!;
        public List<ItemUnit> Units { get; set; } = new List<ItemUnit>();
    }
}
