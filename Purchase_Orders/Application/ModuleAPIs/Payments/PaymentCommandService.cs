using Purchase_Orders.Application.Contracts.Payments;
using Purchase_Orders.Application.Contracts.Payments.Dtos;
using Purchase_Orders.Application.IRepositories.Payments;
using Purchase_Orders.Domain.Payments.Enums;
using Purchase_Orders.Domain.Payments;
using Purchase_Orders.Domain.PurchaseOrders;

namespace Purchase_Orders.Application.ModuleAPIs.Payments
{
    public class PaymentCommandService : IPaymentCommandService
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentCommandService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public void CreateAdvancePayment(AdvancePaymentCreationSnapshotDto paymentDto)
        {
            var payment = new PurchaseOrderPayment
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = PaymentType.Advance,
                Amount = paymentDto.Amount,
                AdvanceDeduction = 0,
                CurrencyId = paymentDto.CurrencyId,
                CurrencyFactor = paymentDto.CurrencyFactor,
                PurchaseOrderId = paymentDto.PurchaseOrderId,
                PurchaseOrderStatementId = null
            };

            _paymentRepository.Add(payment);
        }

        public void CreateRetentionPayment(RetentionPaymentCreationSnapshotDto paymentDto)
        {
            var payment = new PurchaseOrderPayment
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = PaymentType.Retention,
                Amount = paymentDto.Amount,
                AdvanceDeduction = 0,
                CurrencyId = paymentDto.CurrencyId,
                CurrencyFactor = paymentDto.CurrencyFactor,
                PurchaseOrderId = paymentDto.PurchaseOrderId,
                PurchaseOrderStatementId = null
            };

            _paymentRepository.Add(payment);
        }
    }
}
