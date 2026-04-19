using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Application.IRepositories.PurchaseOrderStatements
{
    public interface IPurchaseOrderStatementRepository
    {
        void Add(PurchaseOrderStatement statement);
        Task<PurchaseOrderStatement?> GetByIdAsync(Guid id);
        Task<PurchaseOrderStatement?> GetByIdWithItemsAndViolationsAsync(Guid id);
    }
}