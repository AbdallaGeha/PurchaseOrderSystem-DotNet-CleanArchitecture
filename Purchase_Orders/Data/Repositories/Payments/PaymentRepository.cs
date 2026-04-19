using Purchase_Orders.Application.IRepositories.Payments;
using Purchase_Orders.Domain.Payments;

namespace Purchase_Orders.Data.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;
        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(PurchaseOrderPayment payment)
        {
            _context.PurchaseOrderPayments.Add(payment);
        }
    }
}
