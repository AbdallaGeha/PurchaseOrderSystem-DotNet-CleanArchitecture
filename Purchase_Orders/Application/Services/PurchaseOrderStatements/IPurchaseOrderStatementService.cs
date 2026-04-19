using Purchase_Orders.Application.Dtos.PurchaseOrderStatements;

namespace Purchase_Orders.Application.Services.PurchaseOrderStatements
{
    /// <summary>
    /// Defines business operations for managing purchase order statements.
    /// </summary>
    /// <remarks>
    /// A purchase order statement represents a periodic financial and
    /// quantity declaration against a purchase order.
    /// This service handles creation, retrieval, approval, and updates
    /// of statements, including quantities and violation discounts.
    /// </remarks>
    public interface IPurchaseOrderStatementService
    {
        /// <summary>
        /// Creates a new purchase order statement.
        /// </summary>
        Task AddStatement(StatementCreationDto statementDto);

        /// <summary>
        /// Approves the specified purchase order statement,
        /// after approval, we are allowed to insert payments
        /// </summary>
        Task Approve(Guid statementId);

        /// <summary>
        /// Approves the quantities of the specified statement
        /// and updates inventory
        /// </summary>
        Task ApproveQuantity(Guid statementId);

        /// <summary>
        /// Determines whether a new statement can be created
        /// for the specified purchase order.
        /// </summary>
        Task<bool> CanGetNewStatement(Guid purchaseOrderId);

        /// <summary>
        /// Gets a pre-filled DTO for creating a new statement
        /// for the specified purchase order.
        /// </summary>
        Task<NewStatementDto> GetNewStatement(Guid purchaseOrderId);

        /// <summary>
        /// Retrieves an existing purchase order statement by its identifier.
        /// </summary>
        Task<StatementDto> GetStatementById(Guid statementId);

        /// <summary>
        /// Updates an existing purchase order statement.
        /// </summary>
        Task UpdateStatement(Guid statementId, StatementUpdateDto statementDto);
    }
}