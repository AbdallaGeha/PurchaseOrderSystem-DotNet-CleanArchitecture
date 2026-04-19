using Purchase_Orders.Application.Dtos.Setup;

namespace Purchase_Orders.Application.IQueries.Setup
{
    public interface ILookupQuery
    {
        Task<List<KeyValueDto>> GetCurrenciesLookup();
        Task<List<KeyValueDto>> GetItemsLookup();
        Task<List<KeyValueDto>> GetProjectsLookup();
        Task<List<KeyValueDto>> GetSuppliersLookup();
        Task<List<KeyValueDto>> GetUnitsLookup(Guid itemId);
    }
}