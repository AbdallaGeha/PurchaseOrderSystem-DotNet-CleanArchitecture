using Purchase_Orders.Application.Dtos.PurchaseOrders;

namespace Purchase_Orders.Application.Services.PurchaseOrders
{
    /// <summary>
    /// Defines business operations for managing purchase orders.
    /// </summary>
    /// <remarks>
    /// This service encapsulates all purchase order–related rules,
    /// including creation, updates, approval workflow, and closure.
    /// It is consumed by API controllers and other
    /// application-layer services.
    /// </remarks>
    public interface IPurchaseOrderService
    {
        /// <summary>
        /// Creates a new purchase order.
        /// </summary>
        Task AddPurchaseOrderAsync(PurchaseOrderCreationDto purchaseOrderDto);

        /// <summary>
        /// Approves the specified purchase order, 
        /// once approved, we can add new statements
        /// </summary>
        Task ApproveAsync(Guid id);

        /// <summary>
        /// Check if specified purchase order can be approved.
        /// </summary>
        Task<bool> CanApproveAsync(Guid id);

        /// <summary>
        /// Determines whether the specified purchase order can be updated.
        /// Used by the UI to enable or disable editing based on
        /// business rules 
        /// </summary>
        Task<bool> CanUpdateAsync(Guid id);

        /// <summary>
        /// Closes the specified purchase order indicating that no further
        /// statements, payments, or changes are allowed.
        /// </summary>
        Task CloseAsync(Guid id);

        /// <summary>
        /// Gets the next available purchase order number for a given year.
        /// </summary>
        Task<int> GetNumber(int year);

        /// <summary>
        /// Gets a purchase order with its items
        /// for update or display purposes.
        /// </summary>   
        Task<PurchaseOrderUpdateGetDto> GetPurchaseOrderWithItemsByIdAsync(Guid id);

        /// <summary>
        /// Updates an existing purchase order.
        /// </summary>
        Task UpdatePurchaseOrderAsync(Guid id, PurchaseOrderUpdateDto purchaseOrderDto);
    }
}