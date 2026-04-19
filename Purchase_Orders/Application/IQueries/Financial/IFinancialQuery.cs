using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Application.IQueries.Financial
{
    public interface IFinancialQuery
    {
        Task<decimal?> GetAdvancePaymentAmount(Guid purchaseOrderId);
        Task<decimal> GetAdvancePaymentDeductionsAmount(Guid purchaseOrderId);
        Task<decimal?> GetApprovedStatementsTotalAmount(Guid purchaseOrderId);
        Task<decimal> GetOriginalRegularPaymentsAmount(Guid purchaseOrderId);
        Task<decimal> GetPaidPaymentsTotal(Guid purchaseOrderId);
        Task<decimal> GetViolationsTotal(PurchaseOrder purchaseOrder);
    }
}