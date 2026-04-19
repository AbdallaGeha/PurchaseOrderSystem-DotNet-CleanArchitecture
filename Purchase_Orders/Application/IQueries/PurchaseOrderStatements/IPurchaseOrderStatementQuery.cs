using Purchase_Orders.Application.Dtos.PurchaseOrderStatements;
using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Application.IQueries.PurchaseOrderStatements
{
    public interface IPurchaseOrderStatementQuery
    {
        Task<List<OrderItemQuantityDto>> GetAccumQuantitiesPerOrderItemAsync(List<Guid> statementIds);
        Task<PurchaseOrderStatement?> GetByIdWithItemsAndViolationsAsync(Guid id);
        Task<List<ApproveQuantityInventoryDto>> GetDetailsForInventoryAsync(Guid statementId);
        Task<List<Guid>> GetIdsAsync(Guid purchaseOrderId);
        Task<List<Guid>> GetIdsExceptAsync(Guid purchaseOrderId, Guid statementId);
        Task<List<Guid>> GetIdsUpToStatementNumberAsync(Guid purchaseOrderId, int statementNumber);
        Task<Guid?> GetLastApprovedStatementIdAsync(Guid purchaseOrderId);
        Task<int?> GetLastNumberAsync(Guid purchaseOrderId);
        Task<NewStatementDto?> GetBasicNewStatementAsync(Guid purchaseOrderId);
        Task<PurchaseOrderSnapshotDto?> GetPurchaseOrderSnapshotAsync(Guid purchaseOrderId);
    }
}