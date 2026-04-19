using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Purchase_Orders.Application.Dtos.Setup;
using Purchase_Orders.Application.IQueries.Setup;

namespace Purchase_Orders.API.Controllers
{
    [Route("api/lookup")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupQuery _query;

        public LookupController(ILookupQuery lookupQuery)
        {
            _query = lookupQuery;
        }

        /// <summary>
        /// Retrieves a list of projects key value pairs
        /// </summary>
        /// <response code="200">a list of suppliers</response>
        [HttpGet("projects")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetProjects()
        {
            var projects = await _query.GetProjectsLookup();
            return Ok(projects);
        }

        /// <summary>
        /// Retrieves a list of suppliers key value pairs
        /// </summary>
        /// <response code="200">a list of suppliers</response>
        [HttpGet("suppliers")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetSuppliers()
        {
            var suppliers = await _query.GetSuppliersLookup();
            return Ok(suppliers);
        }

        /// <summary>
        /// Retrieves a list of expenses items key value pairs
        /// </summary>
        /// <response code="200">a list of items </response>
        [HttpGet("items")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetItems()
        {
            var items = await _query.GetItemsLookup();
            return Ok(items);
        }

        /// <summary>
        /// Retrieves a list of expenses Units key value pairs
        /// </summary>
        /// <response code="200">a list of Units </response>
        [HttpGet("Units/{id}")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetUnits(Guid id)
        {
            var Units = await _query.GetUnitsLookup(id);
            return Ok(Units);
        }

        /// <summary>
        /// Retrieves a list of expenses currencies key value pairs
        /// </summary>
        /// <response code="200">a list of currencies </response>
        [HttpGet("currencies")]
        [ProducesResponseType(typeof(List<KeyValueDto>), 200)]
        public async Task<ActionResult<List<KeyValueDto>>> GetCurrencies()
        {
            var currencies = await _query.GetCurrenciesLookup();
            return Ok(currencies);
        }
    }
}
