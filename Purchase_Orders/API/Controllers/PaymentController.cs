using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchase_Orders.Application.Dtos.Payments;
using Purchase_Orders.Application.Services.Payments;

namespace Purchase_Orders.API.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Checks whether a payment can be inserted for the specified statement.
        /// </summary>
        /// <param name="statementId">The statement identifier.</param>
        /// <returns>
        /// True if a payment can be inserted; otherwise, false.
        /// </returns>
        /// <remarks>
        /// This endpoint is typically used by the UI to enable or disable
        /// the "Insert Payment" action based on business rules.
        /// </remarks>
        [HttpGet("statement/{statementId}/can-insert")]
        public async Task<ActionResult<bool>> CanInsertPayment(Guid statementId)
        {
            var result = await _paymentService.CanInsertPayment(statementId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the data required to insert a new payment
        /// for the specified statement.
        /// </summary>
        /// <param name="statementId">The statement identifier.</param>
        /// <returns>
        /// A DTO containing statement, purchase order, and financial details
        /// required to create a payment.
        /// </returns>
        [HttpGet("statement/{statementId}/insert-details")]
        public async Task<ActionResult<InsertPaymentGetDetailsDto>> GetInsertPaymentDetails(
            Guid statementId)
        {
            var dto = await _paymentService.GetInsertPaymentDetails(statementId);
            return Ok(dto);
        }

        /// <summary>
        /// Retrieves the data required to insert a new payment
        /// for the specified statement and purchase order.
        /// </summary>
        /// <param name="statementId">The statement identifier.</param>
        /// <param name="purchaseOrderId">The purchase order identifier.</param>
        /// <returns>
        /// A DTO containing detailed financial data 
        /// </returns>
        [HttpGet("statement/{statementId}/purchase-order/{purchaseOrderId}/insert-details")]
        public async Task<ActionResult<InsertPaymentGetDetailsDto>> GetInsertPaymentDetails(
            Guid statementId,
            Guid purchaseOrderId)
        {
            var dto = await _paymentService.GetInsertPaymentDetails(statementId);

            return Ok(dto);
        }

        /// <summary>
        /// Inserts a new payment.
        /// </summary>
        /// <param name="paymentDto">
        /// The payment creation data
        /// </param>
        /// <returns>No content if insertion succeeds.</returns>
        /// <remarks>
        /// This operation creates a financial transaction and allowed only after 
        /// statement approval.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> InsertPayment(
            [FromBody] RegularPaymentCreationDto paymentDto)
        {
            await _paymentService.InsertPayment(paymentDto);
            return NoContent();
        }
    }
}
