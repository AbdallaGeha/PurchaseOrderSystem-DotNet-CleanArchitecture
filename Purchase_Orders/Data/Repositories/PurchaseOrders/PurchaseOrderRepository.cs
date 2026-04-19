using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Application.IRepositories.PurchaseOrders;
using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Data.Repositories.PurchaseOrders
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly ApplicationDbContext _context;
        public PurchaseOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
        {
            return await _context.PurchaseOrders
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PurchaseOrder?> GetByIdWithItemsAsync(Guid id)
        {
            return await _context.PurchaseOrders
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public void Add(PurchaseOrder purchaseOrder)
        {
            _context.PurchaseOrders.Add(purchaseOrder);
        }
    }
}
