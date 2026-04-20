using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Application.Dtos.PurchaseOrderStatements;
using Purchase_Orders.Application.IQueries.PurchaseOrderStatements;
using Purchase_Orders.Domain.PurchaseOrders;
using Purchase_Orders.Domain.PurchaseOrders.Enums;

namespace Purchase_Orders.Data.Queries.PurchaseOrderStatements
{
    public class PurchaseOrderStatementQuery : IPurchaseOrderStatementQuery
    {
        private readonly ApplicationDbContext _context;
        public PurchaseOrderStatementQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseOrderStatement?> GetByIdWithItemsAndViolationsAsync(Guid id)
        {
            return await _context.PurchaseOrderStatements
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.Violations)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Guid?> GetLastApprovedStatementIdAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderStatements
                .Where(x => x.PurchaseOrderId == purchaseOrderId && x.State == PurchaseOrderStatementState.Approved)
                .OrderByDescending(x => x.Number)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefaultAsync();
        }

        public async Task<List<Guid>> GetIdsAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderStatements
                .Where(x => x.PurchaseOrderId == purchaseOrderId).Select(x => x.Id).ToListAsync();
        }

        public async Task<List<Guid>> GetIdsExceptAsync(Guid purchaseOrderId, Guid statementId)
        {
            return await _context.PurchaseOrderStatements
                .Where(x => x.PurchaseOrderId == purchaseOrderId && x.Id != statementId)
                .Select(x => x.Id).ToListAsync();
        }

        public async Task<List<Guid>> GetIdsUpToStatementNumberAsync(Guid purchaseOrderId, int statementNumber)
        {
            return await _context.PurchaseOrderStatements
                .Where(x => x.PurchaseOrderId == purchaseOrderId && x.Number <= statementNumber)
                .Select(x => x.Id).ToListAsync();
        }

        public async Task<List<OrderItemQuantityDto>> GetAccumQuantitiesPerOrderItemAsync(List<Guid> statementIds)
        {
            return await _context.PurchaseOrderStatementItems
                .Where(x => statementIds.Contains(x.PurchaseOrderStatementId))
                .GroupBy(x => x.PurchaseOrderItemId)
                .Select(g => new OrderItemQuantityDto
                {
                    PurchaseOrderItemId = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .ToListAsync();
        }

        public async Task<int?> GetLastNumberAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderStatements
                .Where(x => x.PurchaseOrderId == purchaseOrderId)
                .Select(x => (int?)x.Number)
                .MaxAsync();
        }

        public async Task<Guid?> GetPurchaseOrderIdByStatementIdAsync(Guid statementId)
        {
            return await _context.PurchaseOrderStatements
                .Where(x => x.Id == statementId)
                .Select(x => (Guid?)x.PurchaseOrderId)
                .SingleOrDefaultAsync();
        }

        public async Task<NewStatementDto?> GetBasicNewStatementAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrders
                .Where(x => x.Id == purchaseOrderId)
                .Select(x => new NewStatementDto
                {
                    StatementNumber = 0,
                    PoNumber = x.Number,
                    PoDate = x.Date.ToString("dd MMM yyyy"),
                    PoReference = x.Ref,
                    Project = x.Project.Name,
                    Supplier = x.Supplier.Name,
                    Currency = x.Currency.Name,
                    CurrencyFactor = x.CurrencyFactor,
                    Items = x.Items
                        .OrderBy(i => i.LineNo)
                        .Select(i => new NewStatementItemDto
                        {
                            PurchaseOrderItemId = i.Id,
                            LineNo = i.LineNo,
                            Item = i.Item.Name,
                            Unit = i.Unit.Name,
                            QuantityPo = i.Quantity,
                            QuantityAccum = 0,
                            UnitPrice = i.UnitPrice,
                            DiscountPercent = i.DiscountPercent ?? x.DiscountPercent,
                            AmountPo = i.Quantity * i.UnitPrice * (i.DiscountPercent ?? x.DiscountPercent),
                            AmountAccum = 0
                        })
                        .ToList()
                })
                .SingleOrDefaultAsync();
        }

        public async Task<List<ApproveQuantityInventoryDto>> GetDetailsForInventoryAsync(Guid statementId)
        {
            return await _context.PurchaseOrderStatementItems
                .Where(si => si.PurchaseOrderStatementId == statementId)
                .Join(_context.PurchaseOrderItems,
                    si => si.PurchaseOrderItemId,
                    poi => poi.Id,
                    (si, poi) => new ApproveQuantityInventoryDto
                    {
                        StatementItemId = si.Id,
                        Quantity = si.Quantity,
                        ItemId = poi.ItemId,
                        UnitId = poi.UnitId
                    })
                .ToListAsync();
        }

        public async Task<decimal?> GetApprovedStatementsTotalAmount(Guid purchaseOrderId)
        {
            var total = await
                _context.PurchaseOrderStatements
                .Where(x => x.PurchaseOrderId == purchaseOrderId && x.State == PurchaseOrderStatementState.Approved)
                .SelectMany(x => x.Items, (s, si) => new { s, si })
                .Join(_context.PurchaseOrderItems,
                    c1 => c1.si.PurchaseOrderItemId,
                    c2 => c2.Id,
                    (c1, c2) => new { c1.s, c1.si, poi = c2 }
                )
                .Join(_context.PurchaseOrders,
                c1 => c1.poi.PurchaseOrderId,
                c2 => c2.Id,
                (c1, c2) => new { c1.s, c1.si, c1.poi, po = c2 })
                .SumAsync(x => x.si.Quantity * x.poi.UnitPrice * (1 - (x.poi.DiscountPercent ?? x.po.DiscountPercent)));

            var violations = await
                _context.PurchaseOrderStatements
                .Where(x => x.PurchaseOrderId == purchaseOrderId && x.State == PurchaseOrderStatementState.Approved)
                .Join(_context.PurchaseOrderViolationDiscounts,
                c1 => c1.Id,
                c2 => c2.PurchaseOrderStatementId,
                (c1, c2) => c2.Amount)
                .SumAsync();

            return total - violations;
        }

        public async Task<decimal> GetViolationsTotal(PurchaseOrder purchaseOrder)
        {
            //var currencyFactor = purchaseOrder.CurrencyFactor;

            var statementsIds = await _context.PurchaseOrderStatements
                .Where(x => x.PurchaseOrderId == purchaseOrder.Id)
                .Select(x => x.Id)
                .ToListAsync();

            var violationsOriginalCurrency = await _context.PurchaseOrderViolationDiscounts
                .Where(x => statementsIds.Contains(x.PurchaseOrderStatementId))
                .SumAsync(x => x.Amount);

            return violationsOriginalCurrency;
        }
    }
}
