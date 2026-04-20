using Purchase_Orders.Application.Contracts.PurchaseOrders.Dtos;
using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Application.Contracts.PurchaseOrders
{
    /// <summary>
    /// This is a contract to be used by other modules to get Po related info
    /// </summary>
    public interface IPurchaseOrderReadService
    {
        Task<Guid?> GetIdByStatementIdAsync(Guid statementId);
        Task<CurrencySnapshotDto?> GetCurrencyDetailsByIdAsync(Guid id);
        Task<Guid?> GetLastApprovedStatementIdAsync(Guid purchaseOrderId);
        Task<decimal?> GetApprovedStatementsTotalAmountAsync(Guid purchaseOrderId);
    }
}
