using Purchase_Orders.Application.Dtos.PurchaseOrders;
using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Application.IQueries.PurchaseOrders
{
    public interface IPurchaseOrderQuery
    {
        Task<PurchaseOrder?> GetByIdAsync(Guid id);
        Task<PurchaseOrder?> GetByIdWithItemsAsync(Guid id);
        Task<int?> GetLastNumberAsync(int year);
        Task<PurchaseOrderDetailDto?> GetPurchaseOrderDetailAsync(Guid purchaseOrderId);
        Task<Guid?> GetStoreIdByIdAsync(Guid id);
    }
}