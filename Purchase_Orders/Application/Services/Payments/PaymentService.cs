using Purchase_Orders.Application.Common.Exceptions;
using Purchase_Orders.Application.Dtos.Payments;
using Purchase_Orders.Application.IRepositories.Payments;
using Purchase_Orders.Application.IUOW;
using Purchase_Orders.Domain.Payments.Enums;
using Purchase_Orders.Domain.Payments;
using Purchase_Orders.Application.IQueries.Payments;
using Purchase_Orders.Application.Contracts.PurchaseOrders;

namespace Purchase_Orders.Application.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentQuery _paymentQuery;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPurchaseOrderReadService _purchaseOrderReadService;
        private readonly IUnitOfWork _uow;
        public PaymentService(
            IPaymentQuery paymentQuery,
            IPaymentRepository paymentRepository,
            IPurchaseOrderReadService purchaseOrderReadService,
            IUnitOfWork uow)
        {
            _paymentQuery = paymentQuery;
            _paymentRepository = paymentRepository;
            _purchaseOrderReadService = purchaseOrderReadService;
            _uow = uow;
        }

        public async Task<bool> CanInsertPayment(Guid statementId)
        {
            var purchaseOrderId = await _purchaseOrderReadService.GetIdByStatementIdAsync(statementId);

            if (purchaseOrderId == null)
                throw new NotFoundException("purchase order not found.");

            var lastStatementId = await _purchaseOrderReadService.GetLastApprovedStatementIdAsync(purchaseOrderId.Value);

            if (lastStatementId == null || lastStatementId.Value != statementId)
                return false;

            var total = await _purchaseOrderReadService.GetApprovedStatementsTotalAmountAsync(purchaseOrderId.Value);
            if (!total.HasValue)
                return false;

            var payments = await _paymentQuery.GetOriginalRegularPaymentsAmountAsync(purchaseOrderId.Value);

            return total.Value - payments > 0.1m;
        }

        public async Task InsertPayment(RegularPaymentCreationDto paymentDto)
        {
            if (paymentDto.AdvanceDeduction >= paymentDto.Amount)
                throw new BusinessException("Payment amount should be greater than Advance deduction.");

            var purchaseOrderId = await _purchaseOrderReadService.GetIdByStatementIdAsync(paymentDto.PurchaseOrderStatementId);

            if (purchaseOrderId == null)
                throw new NotFoundException("purchase order not found.");

            var currencyDetails = await _purchaseOrderReadService.GetCurrencyDetailsByIdAsync(purchaseOrderId.Value);

            if (currencyDetails == null)
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
                CurrencyId = currencyDetails.CurrencyId,
                CurrencyFactor = currencyDetails.CurrencyFactor,
                PurchaseOrderId = purchaseOrderId.Value,
                PurchaseOrderStatementId = paymentDto.PurchaseOrderStatementId
            };

            _paymentRepository.Add(payment);
            await _uow.SaveChangesAsync();
        }

        public async Task<InsertPaymentGetDetailsDto> GetInsertPaymentDetails(Guid statementId)
        {
            var purchaseOrderId = await _purchaseOrderReadService.GetIdByStatementIdAsync(statementId);

            if (purchaseOrderId == null)
                throw new NotFoundException("purchase order not found.");

            var total = await _purchaseOrderReadService.GetApprovedStatementsTotalAmountAsync(purchaseOrderId.Value);
            if (!total.HasValue)
                throw new BusinessException("approved statements amount is zero");

            var payments = await _paymentQuery.GetOriginalRegularPaymentsAmountAsync(purchaseOrderId.Value);

            var advancePayment = await _paymentQuery.GetAdvancePaymentAmountAsync(purchaseOrderId.Value);
            var advanceDeductions = await _paymentQuery.GetAdvancePaymentDeductionsAmountAsync(purchaseOrderId.Value);

            return new InsertPaymentGetDetailsDto
            {
                AllowedPayment = total.Value - payments,
                AllowedAdvanceDeduction = advancePayment.HasValue ? advancePayment.Value - advanceDeductions : 0,
            };
        }
    }
}
