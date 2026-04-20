using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Application.Dtos.PurchaseOrders;
using Purchase_Orders.Application.Dtos.PurchaseOrderStatements;
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

        public async Task<PurchaseOrderDetailDto?> GetPurchaseOrderDetailAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrders
                .Where(x => x.Id == purchaseOrderId)
                .Select(x => new PurchaseOrderDetailDto
                {
                    Number = x.Number,
                    Date = x.Date,
                    Reference = x.Ref,
                    Project = x.Project.Name,
                    Supplier = x.Supplier.Name,
                    Currency = x.Currency.Name,
                    CurrencyFactor = x.CurrencyFactor,
                    DiscountPercent = x.DiscountPercent,
                    Items = x.Items.Select(i => new PurchaseOrderItemDetailDto
                    {
                        Id = i.Id,
                        LineNo = i.LineNo,
                        Item = i.Item.Name,
                        Unit = i.Unit.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        DiscountPercent = i.DiscountPercent
                    }).ToList()
                })
                .SingleOrDefaultAsync();
        }
    }
}
