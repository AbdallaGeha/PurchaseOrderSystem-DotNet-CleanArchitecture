using Purchase_Orders.Domain.Payments;

namespace Purchase_Orders.Application.IRepositories.Payments
{
    public interface IPaymentRepository
    {
        void Add(PurchaseOrderPayment payment);
    }
}