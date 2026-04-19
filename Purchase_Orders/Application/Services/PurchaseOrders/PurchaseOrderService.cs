using AutoMapper;
using Purchase_Orders.Application.Common.Exceptions;
using Purchase_Orders.Application.Dtos.PurchaseOrders;
using Purchase_Orders.Application.IQueries.Financial;
using Purchase_Orders.Application.IQueries.PurchaseOrders;
using Purchase_Orders.Application.IRepositories.PurchaseOrders;
using Purchase_Orders.Application.IUOW;
using Purchase_Orders.Domain.Payments.Enums;
using Purchase_Orders.Domain.Payments;
using Purchase_Orders.Domain.PurchaseOrders;
using Purchase_Orders.Application.IRepositories.Payments;

namespace Purchase_Orders.Application.Services.PurchaseOrders
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderQuery _purchaseOrderQuery;
        private readonly IFinancialQuery _financialQuery;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderQuery purchaseOrderQuery,
            IFinancialQuery financialQuery, IPurchaseOrderRepository purchaseOrderRepository,
            IPaymentRepository paymentRepository,
            IUnitOfWork uow, IMapper mapper)
        {
            _purchaseOrderQuery = purchaseOrderQuery;
            _financialQuery = financialQuery;
            _purchaseOrderRepository = purchaseOrderRepository;
            _paymentRepository = paymentRepository;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task AddPurchaseOrderAsync(PurchaseOrderCreationDto purchaseOrderDto)
        {
            var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderDto);
            _purchaseOrderRepository.Add(purchaseOrder);
            await _uow.SaveChangesAsync();
        }

        public async Task<bool> CanUpdateAsync(Guid id)
        {
            var purchaseOrder = await _purchaseOrderQuery.GetByIdAsync(id);

            if (purchaseOrder == null)
                throw new NotFoundException("Purchase order not found.");

            if (purchaseOrder.State == Domain.PurchaseOrders.Enums.PurchaseOrderState.Created)
                return true;

            return false;
        }

        public async Task UpdatePurchaseOrderAsync(Guid id, PurchaseOrderUpdateDto purchaseOrderDto)
        {
            var noDuplication = purchaseOrderDto.Items
                .GroupBy(x => new { x.ItemId, x.UnitId })
                .All(g => g.Count() == 1);

            if (!noDuplication)
                throw new BusinessException("duplicate item and unit in more than one row");

            var purchaseOrder = await _purchaseOrderRepository.GetByIdWithItemsAsync(id);
            _mapper.Map(purchaseOrderDto, purchaseOrder);
            await _uow.SaveChangesAsync();
        }

        public async Task<bool> CanApproveAsync(Guid id)
        {
            var purchaseOrder = await _purchaseOrderQuery.GetByIdAsync(id);

            if (purchaseOrder == null)
                throw new NotFoundException("Purchase order not found.");

            if (purchaseOrder.State != Domain.PurchaseOrders.Enums.PurchaseOrderState.Created)
                return false;

            return true;
        }

        public async Task ApproveAsync(Guid id)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(id);

            if (purchaseOrder == null)
                throw new NotFoundException("Purchase order not found.");

            if (purchaseOrder.State != Domain.PurchaseOrders.Enums.PurchaseOrderState.Created)
                throw new BusinessException("Purchase Order should be in created state before approval");

            purchaseOrder.State = Domain.PurchaseOrders.Enums.PurchaseOrderState.Approved;

            var payment = new PurchaseOrderPayment
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = PaymentType.Advance,
                Amount = purchaseOrder.AdvancePayment,
                AdvanceDeduction = 0,
                CurrencyId = purchaseOrder.CurrencyId,
                CurrencyFactor = purchaseOrder.CurrencyFactor,
                PurchaseOrderId = purchaseOrder.Id,
                PurchaseOrderStatementId = null
            };

            _paymentRepository.Add(payment);
            await _uow.SaveChangesAsync();
        }

        public async Task CloseAsync(Guid id)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetByIdWithItemsAsync(id);

            if (purchaseOrder == null)
                throw new NotFoundException("Purchase order not found.");

            if (purchaseOrder.State != Domain.PurchaseOrders.Enums.PurchaseOrderState.Approved)
                throw new BusinessException("Purchase Order should be in Approved state before close");

            var orderTotal = purchaseOrder.GetTotal();
            var paidTotal = await _financialQuery.GetPaidPaymentsTotal(purchaseOrder.Id);
            var violationTotal = await _financialQuery.GetViolationsTotal(purchaseOrder);
            var retentionAmount = purchaseOrder.GetRetentionAmount();

            if (orderTotal != paidTotal + violationTotal + retentionAmount)
                throw new BusinessException("Purchase Order is not ready to close");

            purchaseOrder.State = Domain.PurchaseOrders.Enums.PurchaseOrderState.Closed;

            var retentionPayment = new PurchaseOrderPayment
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Type = PaymentType.Retention,
                Amount = retentionAmount,
                AdvanceDeduction = 0,
                CurrencyId = purchaseOrder.CurrencyId,
                CurrencyFactor = purchaseOrder.CurrencyFactor,
                PurchaseOrderId = purchaseOrder.Id,
                PurchaseOrderStatementId = null
            };

            _paymentRepository.Add(retentionPayment);

            await _uow.SaveChangesAsync();
        }
        public async Task<PurchaseOrderUpdateGetDto> GetPurchaseOrderWithItemsByIdAsync(Guid id)
        {
            var purchaseOrder = await _purchaseOrderQuery.GetByIdWithItemsAsync(id);

            if (purchaseOrder == null)
                throw new NotFoundException("purchase order not found.");

            return _mapper.Map<PurchaseOrderUpdateGetDto>(purchaseOrder);
        }
        public async Task<int> GetNumber(int year)
        {
            var lastNumber = await _purchaseOrderQuery.GetLastNumberAsync(year);
            return (lastNumber ?? 0) + 1;
        }
    }
}
