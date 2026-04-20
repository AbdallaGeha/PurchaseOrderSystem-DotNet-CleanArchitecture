using Purchase_Orders.Application.Contracts.Payments.Dtos;

namespace Purchase_Orders.Application.Contracts.Payments
{
    /// <summary>
    /// This is a contract to be used by other modules to execute Payment commands
    /// </summary>
    public interface IPaymentCommandService
    {
        void CreateAdvancePayment(AdvancePaymentCreationSnapshotDto payment);
        void CreateRetentionPayment(RetentionPaymentCreationSnapshotDto payment);
    }
}
