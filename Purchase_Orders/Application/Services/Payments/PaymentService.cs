using Purchase_Orders.Application.Common.Exceptions;
using Purchase_Orders.Application.Dtos.Payments;
using Purchase_Orders.Application.IQueries.Financial;
using Purchase_Orders.Application.IQueries.PurchaseOrders;
using Purchase_Orders.Application.IQueries.PurchaseOrderStatements;
using Purchase_Orders.Application.IRepositories.Payments;
using Purchase_Orders.Application.IUOW;
using Purchase_Orders.Domain.Payments.Enums;
using Purchase_Orders.Domain.Payments;

namespace Purchase_Orders.Application.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPurchaseOrderQuery _purchaseOrderQuery;
        private readonly IPurchaseOrderStatementQuery _purchaseOrderStatementQuery;
        private readonly IFinancialQuery _financialQuery;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _uow;
        public PaymentService(IPurchaseOrderQuery purchaseOrderQuery,
            IPurchaseOrderStatementQuery purchaseOrderStatementQuery,
            IFinancialQuery financialQuery,
            IPaymentRepository paymentRepository,
            IUnitOfWork uow)
        {
            _purchaseOrderQuery = purchaseOrderQuery;
            _purchaseOrderStatementQuery = purchaseOrderStatementQuery;
            _financialQuery = financialQuery;
            _paymentRepository = paymentRepository;
            _uow = uow;
        }

        public async Task<bool> CanInsertPayment(Guid statementId)
        {
            var purchaseOrderId = await _purchaseOrderQuery.GetIdByStatementIdAsync(statementId);

            if (purchaseOrderId == null)
                throw new NotFoundException("purchase order not found.");

            var lastStatementId = await _purchaseOrderStatementQuery.GetLastApprovedStatementIdAsync(purchaseOrderId.Value);

            if (lastStatementId == null || lastStatementId.Value != statementId)
                return false;

            var total = await _financialQuery.GetApprovedStatementsTotalAmount(purchaseOrderId.Value);
            if (!total.HasValue)
                return false;

            var payments = await _financialQuery.GetOriginalRegularPaymentsAmount(purchaseOrderId.Value);

            return total.Value - payments > 0.1m;
        }

        public async Task InsertPayment(RegularPaymentCreationDto paymentDto)
        {
            if (paymentDto.AdvanceDeduction >= paymentDto.Amount)
                throw new BusinessException("Payment amount should be greater than Advance deduction.");

            var purchaseOrderId = await _purchaseOrderQuery.GetIdByStatementIdAsync(paymentDto.PurchaseOrderStatementId);

            if (purchaseOrderId == null)
                throw new NotFoundException("purchase order not found.");

            var purchaseOrder = await _purchaseOrderQuery.GetByIdAsync(purchaseOrderId.Value);

            if (purchaseOrder == null)
                throw new NotFoundException("purchase order not found.");

            var allowedDetails = await GetInsertPaymentDetails(paymentDto.PurchaseOrderStatementId);

            if (paymentDto.Amount <= 0)
            {
                throw new BusinessException("Payment amount should be greater than 0.");
            }

            if (paymentDto.Amount - allowedDetails.AllowedPayment > 0.01m)
            {
                throw new BusinessException("Payment amount exceeds allowed payment.");
            }

            if (paymentDto.AdvanceDeduction - allowedDetails.AllowedAdvanceDeduction > 0.01m)
            {
                throw new BusinessException("Advance Payment deduction amount exceeds allowed.");
            }

            var payment = new PurchaseOrderPayment
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = PaymentType.Regular,
                Amount = paymentDto.Amount,
                AdvanceDeduction = paymentDto.AdvanceDeduction,
                CurrencyId = purchaseOrder.CurrencyId,
                CurrencyFactor = purchaseOrder.CurrencyFactor,
                PurchaseOrderId = purchaseOrderId.Value,
                PurchaseOrderStatementId = paymentDto.PurchaseOrderStatementId
            };

            _paymentRepository.Add(payment);
            await _uow.SaveChangesAsync();
        }

        public async Task<InsertPaymentGetDetailsDto> GetInsertPaymentDetails(Guid statementId)
        {
            var purchaseOrderId = await _purchaseOrderQuery.GetIdByStatementIdAsync(statementId);

            if (purchaseOrderId == null)
                throw new NotFoundException("purchase order not found.");

            var total = await _financialQuery.GetApprovedStatementsTotalAmount(purchaseOrderId.Value);
            if (!total.HasValue)
                throw new BusinessException("approved statements amount is zero");

            var payments = await _financialQuery.GetOriginalRegularPaymentsAmount(purchaseOrderId.Value);

            var advancePayment = await _financialQuery.GetAdvancePaymentAmount(purchaseOrderId.Value);
            var advanceDeductions = await _financialQuery.GetAdvancePaymentDeductionsAmount(purchaseOrderId.Value);

            return new InsertPaymentGetDetailsDto
            {
                AllowedPayment = total.Value - payments,
                AllowedAdvanceDeduction = advancePayment.HasValue ? advancePayment.Value - advanceDeductions : 0,
            };
        }
    }
}
