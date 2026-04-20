using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Application.IQueries.Payments;
using Purchase_Orders.Domain.Payments.Enums;

namespace Purchase_Orders.Data.Queries.Payments
{
    public class PaymentQuery : IPaymentQuery
    {
        private readonly ApplicationDbContext _context;
        public PaymentQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetPaidPaymentsTotalAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderPayments
                .Where(x => x.PurchaseOrderId == purchaseOrderId)
                .SumAsync(x => (x.Amount - x.AdvanceDeduction));
        }

        public async Task<decimal?> GetAdvancePaymentAmountAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderPayments
                .Where(x => x.PurchaseOrderId == purchaseOrderId && x.Type == PaymentType.Advance)
                .Select(x => (decimal?)x.Amount)
                .SingleOrDefaultAsync();
        }

        public async Task<decimal> GetAdvancePaymentDeductionsAmountAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderPayments
                .Where(x => x.PurchaseOrderId == purchaseOrderId && x.Type == PaymentType.Regular)
                .SumAsync(x => x.AdvanceDeduction);
        }

        public async Task<decimal> GetOriginalRegularPaymentsAmountAsync(Guid purchaseOrderId)
        {
            return await _context.PurchaseOrderPayments
                .Where(x => x.PurchaseOrderId == purchaseOrderId && x.Type == PaymentType.Regular)
                .SumAsync(x => x.Amount);
        }
    }
}
