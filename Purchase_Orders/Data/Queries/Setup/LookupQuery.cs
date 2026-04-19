using Microsoft.EntityFrameworkCore;
using Purchase_Orders.Application.Dtos.Setup;
using Purchase_Orders.Application.IQueries.Setup;

namespace Purchase_Orders.Data.Queries.Setup
{
    public class LookupQuery : ILookupQuery
    {
        private readonly ApplicationDbContext _context;
        public LookupQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KeyValueDto>> GetItemsLookup()
        {
            return await _context.Items
                .Select(x => new KeyValueDto { Key = x.Id.ToString().ToLower(), Value = x.Name })
                .ToListAsync();
        }

        public async Task<List<KeyValueDto>> GetUnitsLookup(Guid itemId)
        {
            return await _context.ItemUnits
                .Include(x => x.Unit)
                .Where(x => x.ItemId == itemId)
                .Select(x => new KeyValueDto { Key = x.UnitId.ToString().ToLower(), Value = x.Unit.Name })
                .ToListAsync();
        }

        public async Task<List<KeyValueDto>> GetCurrenciesLookup()
        {
            return await _context.Currencies
                .Select(x => new KeyValueDto { Key = x.Id.ToString().ToLower(), Value = x.Name })
                .ToListAsync();
        }

        public async Task<List<KeyValueDto>> GetProjectsLookup()
        {
            return await _context.Projects
                .Select(x => new KeyValueDto { Key = x.Id.ToString().ToLower(), Value = x.Name })
                .ToListAsync();
        }

        public async Task<List<KeyValueDto>> GetSuppliersLookup()
        {
            return await _context.Suppliers
                .Select(x => new KeyValueDto { Key = x.Id.ToString().ToLower(), Value = x.Name })
                .ToListAsync();
        }
    }
}
