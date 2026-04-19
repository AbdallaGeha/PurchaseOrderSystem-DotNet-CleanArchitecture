using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchase_Orders.Application.Dtos.PurchaseOrders;
using Purchase_Orders.Application.Services.PurchaseOrders;

namespace Purchase_Orders.API.Controllers
{
    [Route("api/purchaseorder")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderservice;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderservice)
        {
            _purchaseOrderservice = purchaseOrderservice;
        }

        /// <summary>
        /// Creates a new purchase order.
        /// </summary>
        /// <param name="poDto">Purchase order creation data.</param>
        /// <returns>No content if creation succeeds.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(PurchaseOrderCreationDto poDto)
        {
            await _purchaseOrderservice.AddPurchaseOrderAsync(poDto);
            return NoContent();
        }

        /// <summary>
        /// Gets the next purchase order number for a given year.
        /// </summary>
        /// <param name="year">The year for which the next number is required.</param>
        /// <returns>The next available purchase order number.</returns>
        [HttpGet("next-number/{year:int}")]
        public async Task<ActionResult<int>> GetNextNumber(int year)
        {
            var number = await _purchaseOrderservice.GetNumber(year);
            return Ok(number);
        }

        /// <summary>
        /// Retrieves a purchase order by its identifier,
        /// including its items for update purposes.
        /// </summary>
        /// <param name="id">The purchase order identifier.</param>
        /// <returns>The purchase order data.</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PurchaseOrderUpdateGetDto>> GetById(Guid id)
        {
            var result = await _purchaseOrderservice.GetPurchaseOrderWithItemsByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Checks whether a purchase order can be updated.
        /// </summary>
        /// <param name="id">The purchase order identifier.</param>
        /// <returns>
        /// True if the purchase order can be updated; otherwise, false.
        /// </returns>
        [HttpGet("{id}/can-update")]
        public async Task<ActionResult<bool>> CanUpdate(Guid id)
        {
            var result = await _purchaseOrderservice.CanUpdateAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing purchase order.
        /// </summary>
        /// <param name="id">The purchase order identifier.</param>
        /// <param name="dto">Updated purchase order data.</param>
        /// <returns>No content if update succeeds.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PurchaseOrderUpdateDto dto)
        {
            await _purchaseOrderservice.UpdatePurchaseOrderAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// Checks whether the purchase order can be approved.
        /// </summary>
        /// <param name="id">The purchase order identifier.</param>
        /// <returns>
        /// True if the purchase order can be approved; otherwise, false.
        /// </returns>
        [HttpGet("{id}/can-approve")]
        public async Task<ActionResult<bool>> CanApprove(Guid id)
        {
            var result = await _purchaseOrderservice.CanApproveAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Approves the specified purchase order.
        /// </summary>
        /// <param name="id">The purchase order identifier.</param>
        /// <returns>No content if approval succeeds.</returns>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _purchaseOrderservice.ApproveAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Closes the specified purchase order.
        /// After closing, no further updates or approvals are allowed.
        /// </summary>
        /// <param name="id">The purchase order identifier.</param>
        /// <returns>No content if close succeeds.</returns>
        [HttpPost("{id}/close")]
        public async Task<IActionResult> Close(Guid id)
        {
            await _purchaseOrderservice.CloseAsync(id);
            return NoContent();
        }
    }
}
