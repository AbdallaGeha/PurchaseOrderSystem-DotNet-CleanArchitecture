using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchase_Orders.Application.Dtos.PurchaseOrderStatements;
using Purchase_Orders.Application.Services.PurchaseOrderStatements;

namespace Purchase_Orders.API.Controllers
{
    [Route("api/purchaseorderstatement")]
    [ApiController]
    public class PurchaseOrderStatementController : ControllerBase
    {
        private readonly IPurchaseOrderStatementService _purchaseOrderStatementService;

        public PurchaseOrderStatementController(IPurchaseOrderStatementService purchaseOrderStatementService)
        {
            _purchaseOrderStatementService = purchaseOrderStatementService;
        }

        /// <summary>
        /// Checks whether a new statement can be created for the given purchase order.
        /// </summary>
        /// <param name="purchaseOrderId">The purchase order identifier.</param>
        /// <returns>
        /// True if a new statement can be created; otherwise, false.
        /// </returns>
        [HttpGet("{purchaseOrderId}/can-new")]
        public async Task<ActionResult<bool>> CanGetNewStatement(Guid purchaseOrderId)
        {
            var result = await _purchaseOrderStatementService
                .CanGetNewStatement(purchaseOrderId);

            return Ok(result);
        }

        /// <summary>
        /// Creates a new purchase order statement.
        /// </summary>
        /// <param name="statementDto">
        /// The statement creation data, including quantities and violations.
        /// </param>
        /// <returns>No content if creation succeeds.</returns>
        [HttpPost]
        public async Task<IActionResult> AddStatement(
            [FromBody] StatementCreationDto statementDto)
        {
            await _purchaseOrderStatementService.AddStatement(statementDto);
            return NoContent();
        }

        /// <summary>
        /// Retrieves data required to create a new statement
        /// for a specific purchase order.
        /// </summary>
        /// <param name="purchaseOrderId">The purchase order identifier.</param>
        /// <returns>
        /// A DTO containing purchase order details and accumulated statement data.
        /// </returns>
        [HttpGet("{purchaseOrderId}/new")]
        public async Task<ActionResult<NewStatementDto>> GetNewStatement(Guid purchaseOrderId)
        {
            var dto = await _purchaseOrderStatementService
                .GetNewStatement(purchaseOrderId);

            return Ok(dto);
        }

        /// <summary>
        /// Retrieves an existing purchase order statement by its identifier.
        /// </summary>
        /// <param name="statementId">The statement identifier.</param>
        /// <returns>The statement details.</returns>
        [HttpGet("{statementId}")]
        public async Task<ActionResult<StatementDto>> GetStatementById(Guid statementId)
        {
            var dto = await _purchaseOrderStatementService
                .GetStatementById(statementId);

            return Ok(dto);
        }

        /// <summary>
        /// Approves the specified purchase order statement.
        /// This represents financial approval.
        /// </summary>
        /// <param name="statementId">The statement identifier.</param>
        /// <returns>No content if approval succeeds.</returns>
        [HttpPost("{statementId}/approve")]
        public async Task<IActionResult> Approve(Guid statementId)
        {
            await _purchaseOrderStatementService.Approve(statementId);
            return NoContent();
        }

        /// <summary>
        /// Approves the quantities of the specified purchase order statement.
        /// This represents quantity verification.
        /// </summary>
        /// <param name="statementId">The statement identifier.</param>
        /// <returns>No content if quantity approval succeeds.</returns>
        [HttpPost("{statementId}/approve-quantity")]
        public async Task<IActionResult> ApproveQuantity(Guid statementId)
        {
            await _purchaseOrderStatementService.ApproveQuantity(statementId);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing purchase order statement.
        /// </summary>
        /// <param name="statementId">The statement identifier.</param>
        /// <param name="statementDto">
        /// The updated statement data, including quantities and violations.
        /// </param>
        /// <returns>No content if update succeeds.</returns>
        [HttpPut("{statementId}")]
        public async Task<IActionResult> UpdateStatement(
            Guid statementId,
            [FromBody] StatementUpdateDto statementDto)
        {
            await _purchaseOrderStatementService
                .UpdateStatement(statementId, statementDto);

            return NoContent();
        }
    }
}
