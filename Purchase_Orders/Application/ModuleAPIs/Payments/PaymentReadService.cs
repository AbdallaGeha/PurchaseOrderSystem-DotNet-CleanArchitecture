using Purchase_Orders.Application.Contracts.Payments;
using Purchase_Orders.Application.IQueries.Payments;

namespace Purchase_Orders.Application.ModuleAPIs.Payments
{
    public class PaymentReadService : IPaymentReadService
    {
        private readonly IPaymentQuery _paymentQuery;
        public PaymentReadService(IPaymentQuery paymentQuery)
        {
            _paymentQuery = paymentQuery;
        }

        public async Task<decimal> GetPaidPaymentsTotalAsync(Guid purchaseOrderId)
        {
            return await _paymentQuery.GetPaidPaymentsTotalAsync(purchaseOrderId);
        }
    }
}
