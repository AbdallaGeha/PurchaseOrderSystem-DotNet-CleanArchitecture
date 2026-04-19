using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Application.IRepositories.PurchaseOrders
{
    public interface IPurchaseOrderRepository
    {
        void Add(PurchaseOrder purchaseOrder);
        Task<PurchaseOrder?> GetByIdAsync(Guid id);
        Task<PurchaseOrder?> GetByIdWithItemsAsync(Guid id);
    }
}