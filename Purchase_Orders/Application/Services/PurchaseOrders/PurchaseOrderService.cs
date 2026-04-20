using AutoMapper;
using Purchase_Orders.Application.Common.Exceptions;
using Purchase_Orders.Application.Dtos.PurchaseOrders;
using Purchase_Orders.Application.IQueries.PurchaseOrders;
using Purchase_Orders.Application.IRepositories.PurchaseOrders;
using Purchase_Orders.Application.IUOW;
using Purchase_Orders.Domain.PurchaseOrders;
using Purchase_Orders.Application.IQueries.PurchaseOrderStatements;
using Purchase_Orders.Application.Contracts.Payments;
using Purchase_Orders.Application.Contracts.Payments.Dtos;

namespace Purchase_Orders.Application.Services.PurchaseOrders
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderQuery _purchaseOrderQuery;
        private readonly IPurchaseOrderStatementQuery _purchaseOrderStatementQuery;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPaymentReadService _paymentReadService;
        private readonly IPaymentCommandService _paymentCommandService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderQuery purchaseOrderQuery,
            IPurchaseOrderStatementQuery purchaseOrderStatementQuery,
            IPurchaseOrderRepository purchaseOrderRepository,
            IPaymentReadService paymentReadService, IPaymentCommandService paymentCommandService,
            IUnitOfWork uow, IMapper mapper)
        {
            _purchaseOrderQuery = purchaseOrderQuery;
            _purchaseOrderStatementQuery = purchaseOrderStatementQuery;
            _purchaseOrderRepository = purchaseOrderRepository;
            _paymentReadService = paymentReadService;
            _paymentCommandService = paymentCommandService;
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

            var paymentDto = new AdvancePaymentCreationSnapshotDto
            {
                Amount = purchaseOrder.AdvancePayment,
                CurrencyId = purchaseOrder.CurrencyId,
                CurrencyFactor = purchaseOrder.CurrencyFactor,
                PurchaseOrderId = purchaseOrder.Id,
            };

            _paymentCommandService.CreateAdvancePayment(paymentDto);
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
            var paidTotal = await _paymentReadService.GetPaidPaymentsTotalAsync(purchaseOrder.Id);
            var violationTotal = await _purchaseOrderStatementQuery.GetViolationsTotal(purchaseOrder);
            var retentionAmount = purchaseOrder.GetRetentionAmount();

            if (orderTotal != paidTotal + violationTotal + retentionAmount)
                throw new BusinessException("Purchase Order is not ready to close");

            purchaseOrder.State = Domain.PurchaseOrders.Enums.PurchaseOrderState.Closed;

            var paymentDto = new RetentionPaymentCreationSnapshotDto
            {
                Amount = retentionAmount,
                CurrencyId = purchaseOrder.CurrencyId,
                CurrencyFactor = purchaseOrder.CurrencyFactor,
                PurchaseOrderId = purchaseOrder.Id,
            };

            _paymentCommandService.CreateRetentionPayment(paymentDto);
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
