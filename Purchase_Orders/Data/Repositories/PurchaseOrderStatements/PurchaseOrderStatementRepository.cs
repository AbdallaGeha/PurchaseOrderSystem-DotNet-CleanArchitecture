using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Application.IRepositories.PurchaseOrderStatements;
using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Data.Repositories.PurchaseOrderStatements
{
    public class PurchaseOrderStatementRepository : IPurchaseOrderStatementRepository
    {
        private readonly ApplicationDbContext _context;
        public PurchaseOrderStatementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseOrderStatement?> GetByIdAsync(Guid id)
        {
            return await _context.PurchaseOrderStatements
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PurchaseOrderStatement?> GetByIdWithItemsAndViolationsAsync(Guid id)
        {
            return await _context.PurchaseOrderStatements
                .Include(x => x.Items)
                .Include(x => x.Violations)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public void Add(PurchaseOrderStatement statement)
        {
            _context.PurchaseOrderStatements.Add(statement);
        }
    }
}
