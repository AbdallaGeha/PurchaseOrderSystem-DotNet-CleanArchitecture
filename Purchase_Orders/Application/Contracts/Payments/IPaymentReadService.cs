namespace Purchase_Orders.Application.Contracts.Payments
{
    /// <summary>
    /// This is a contract to be used by other modules to get payment related info
    /// </summary>
    public interface IPaymentReadService
    {
        Task<decimal> GetPaidPaymentsTotalAsync(Guid purchaseOrderId);
    }
}
