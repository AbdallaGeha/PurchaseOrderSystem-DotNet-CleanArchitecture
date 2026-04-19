using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Application.IQueries.PurchaseOrders;
using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Data.Queries.PurchaseOrders
{
    public class PurchaseOrderQuery : IPurchaseOrderQuery
    {
        private readonly ApplicationDbContext _context;
        public PurchaseOrderQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseOrder?> GetByIdAsync(Guid id)
        {
            return await _context.PurchaseOrders
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }
        public async Task<PurchaseOrder?> GetByIdWithItemsAsync(Guid id)
        {
            return await _context.PurchaseOrders
                .Include(x => x.Items)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Guid?> GetIdByStatementIdAsync(Guid statementId)
        {
            return await _context.PurchaseOrderStatements
                .Where(x => x.Id == statementId)
                .Select(x => (Guid?)x.PurchaseOrderId)
                .SingleOrDefaultAsync();
        }
        public async Task<int?> GetLastNumberAsync(int year)
        {
            return await _context.PurchaseOrders
                .Where(x => x.Date.Year == year)
                .Select(x => (int?)x.Number)
                .MaxAsync();
        }

        public async Task<Guid?> GetStoreIdByIdAsync(Guid id)
        {
            return await _context.PurchaseOrders
                .Where(x => x.Id == id)
                .Select(x => (Guid?)x.Project.StoreId)
                .SingleOrDefaultAsync();
        }
    }
}
