using Purchase_Orders.Application.Contracts.PurchaseOrders;
using Purchase_Orders.Application.Contracts.PurchaseOrders.Dtos;
using Purchase_Orders.Application.IQueries.PurchaseOrders;
using Purchase_Orders.Application.IQueries.PurchaseOrderStatements;

namespace Purchase_Orders.Application.ModuleAPIs.PurchaseOrders
{
    public class PurchaseOrderReadService : IPurchaseOrderReadService
    {
        private readonly IPurchaseOrderQuery _purchaseOrderQuery;
        private readonly IPurchaseOrderStatementQuery _purchaseOrderStatementQuery;

        public PurchaseOrderReadService(IPurchaseOrderQuery purchaseOrderQuery,
            IPurchaseOrderStatementQuery purchaseOrderStatementQuery)
        {
            _purchaseOrderQuery = purchaseOrderQuery;
            _purchaseOrderStatementQuery = purchaseOrderStatementQuery;
        }

        public async Task<decimal?> GetApprovedStatementsTotalAmountAsync(Guid purchaseOrderId)
        {
            return await _purchaseOrderStatementQuery.GetApprovedStatementsTotalAmount(purchaseOrderId);
        }

        public async Task<CurrencySnapshotDto?> GetCurrencyDetailsByIdAsync(Guid id)
        {
            var po = await _purchaseOrderQuery.GetByIdAsync(id);
            if (po == null)
                return null;

            return new CurrencySnapshotDto
            {
                CurrencyId = po.CurrencyId,
                CurrencyFactor = po.CurrencyFactor
            };
        }

        public async Task<Guid?> GetIdByStatementIdAsync(Guid statementId)
        {
            return await _purchaseOrderStatementQuery.GetPurchaseOrderIdByStatementIdAsync(statementId);
        }

        public async Task<Guid?> GetLastApprovedStatementIdAsync(Guid purchaseOrderId)
        {
            return await _purchaseOrderStatementQuery.GetLastApprovedStatementIdAsync(purchaseOrderId);
        }
    }
}
