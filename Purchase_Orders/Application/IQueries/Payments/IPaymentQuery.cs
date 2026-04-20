namespace Purchase_Orders.Application.IQueries.Payments
{
    public interface IPaymentQuery
    {
        Task<decimal?> GetAdvancePaymentAmountAsync(Guid purchaseOrderId);
        Task<decimal> GetAdvancePaymentDeductionsAmountAsync(Guid purchaseOrderId);
        Task<decimal> GetOriginalRegularPaymentsAmountAsync(Guid purchaseOrderId);
        Task<decimal> GetPaidPaymentsTotalAsync(Guid purchaseOrderId);
    }
}