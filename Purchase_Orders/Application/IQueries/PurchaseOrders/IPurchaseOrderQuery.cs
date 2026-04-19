using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Application.IQueries.PurchaseOrders
{
    public interface IPurchaseOrderQuery
    {
        Task<PurchaseOrder?> GetByIdAsync(Guid id);
        Task<PurchaseOrder?> GetByIdWithItemsAsync(Guid id);
        Task<Guid?> GetIdByStatementIdAsync(Guid statementId);
        Task<int?> GetLastNumberAsync(int year);
        Task<Guid?> GetStoreIdByIdAsync(Guid id);
    }
}