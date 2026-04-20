using AutoMapper;
using Purchase_Orders.Application.Common.Exceptions;
using Purchase_Orders.Application.Dtos.PurchaseOrderStatements;
using Purchase_Orders.Application.IQueries.PurchaseOrders;
using Purchase_Orders.Application.IQueries.PurchaseOrderStatements;
using Purchase_Orders.Application.IRepositories.PurchaseOrderStatements;
using Purchase_Orders.Application.IUOW;
using Purchase_Orders.Domain.PurchaseOrders.Enums;
using Purchase_Orders.Domain.PurchaseOrders;
using Purchase_Orders.Application.Contracts.Inventory;
using Purchase_Orders.Application.Contracts.Inventory.Dtos;

namespace Purchase_Orders.Application.Services.PurchaseOrderStatements
{
    public class PurchaseOrderStatementService : IPurchaseOrderStatementService
    {
        private readonly IPurchaseOrderQuery _purchaseOrderQuery;
        private readonly IPurchaseOrderStatementQuery _purchaseOrderStatementQuery;
        private readonly IPurchaseOrderStatementRepository _purchaseOrderStatementRepository;
        private readonly IInventoryCommandService _inventoryCommandService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PurchaseOrderStatementService(IPurchaseOrderQuery purchaseOrderQuery,
            IPurchaseOrderStatementQuery purchaseOrderStatementQuery,
            IPurchaseOrderStatementRepository purchaseOrderStatementRepository,
            IInventoryCommandService inventoryCommandService,
            IUnitOfWork uow, IMapper mapper)
        {
            _purchaseOrderQuery = purchaseOrderQuery;
            _purchaseOrderStatementQuery = purchaseOrderStatementQuery;
            _purchaseOrderStatementRepository = purchaseOrderStatementRepository;
            _inventoryCommandService = inventoryCommandService;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<bool> CanGetNewStatement(Guid purchaseOrderId)
        {
            var purchaseOrder = await _purchaseOrderQuery.GetByIdWithItemsAsync(purchaseOrderId);

            if (purchaseOrder == null || purchaseOrder.State != PurchaseOrderState.Approved)
                return false;

            var statementsIds = await _purchaseOrderStatementQuery.GetIdsAsync(purchaseOrderId);

            var accumQuantitesListPerOrderItem = await _purchaseOrderStatementQuery
                .GetAccumQuantitiesPerOrderItemAsync(statementsIds);

            if (purchaseOrder.Items.Count != accumQuantitesListPerOrderItem.Count)
                return true;

            foreach (var item in purchaseOrder.Items)
            {
                var accumItem = accumQuantitesListPerOrderItem.Single(x => x.PurchaseOrderItemId == item.Id);
                if (accumItem.TotalQuantity != item.Quantity)
                    return true;
            }

            return false;
        }

        public async Task<NewStatementDto> GetNewStatement(Guid purchaseOrderId)
        {
            var newStatementDto = await _purchaseOrderStatementQuery
                .GetBasicNewStatementAsync(purchaseOrderId);

            if (newStatementDto == null)
                throw new NotFoundException("Purchase order not found.");

            var maxStatementNumber = await _purchaseOrderStatementQuery.GetLastNumberAsync(purchaseOrderId);
            newStatementDto.StatementNumber = (maxStatementNumber ?? 0) + 1;

            var statementsIds = await _purchaseOrderStatementQuery.GetIdsAsync(purchaseOrderId);
            var accumQuantitesListPerOrderItem = await _purchaseOrderStatementQuery
                .GetAccumQuantitiesPerOrderItemAsync(statementsIds);

            foreach (var itemDto in newStatementDto.Items)
            {
                var accumRow = accumQuantitesListPerOrderItem
                    .SingleOrDefault(x => x.PurchaseOrderItemId == itemDto.PurchaseOrderItemId);

                if (accumRow != null)
                {
                    itemDto.QuantityAccum = accumRow.TotalQuantity;
                    itemDto.AmountAccum = itemDto.QuantityAccum * itemDto.UnitPrice * itemDto.DiscountPercent;
                }
            }

            return newStatementDto;
        }

        public async Task<StatementDto> GetStatementById(Guid statementId)
        {
            var statement = await _purchaseOrderStatementQuery.GetByIdWithItemsAndViolationsAsync(statementId);

            if (statement == null)
                throw new NotFoundException("Purchase order statement not found.");

            var purchaseOrderDetail = await _purchaseOrderQuery
                .GetPurchaseOrderDetailAsync(statement.PurchaseOrderId);

            if (purchaseOrderDetail == null)
                throw new NotFoundException("Purchase order not found.");

            var statementDto = new StatementDto
            {
                Id = statement.Id,
                Number = statement.Number,
                Date = statement.Date,
                Ref = statement.Ref,
                Remarks = statement.Remarks,
                State = (short)statement.State,
                PoNumber = purchaseOrderDetail.Number,
                PoDate = purchaseOrderDetail.Date.ToString("dd MMM yyyy"),
                PoReference = purchaseOrderDetail.Reference,
                Project = purchaseOrderDetail.Project,
                Supplier = purchaseOrderDetail.Supplier,
                Currency = purchaseOrderDetail.Currency,
                CurrencyFactor = purchaseOrderDetail.CurrencyFactor,
                Items = purchaseOrderDetail.Items
                    .OrderBy(i => i.LineNo)
                    .Select(i => new StatementItemDto
                    {
                        Id = null,
                        PurchaseOrderItemId = i.Id,
                        LineNo = i.LineNo,
                        Item = i.Item,
                        Unit = i.Unit,
                        QuantityCurrent = 0,
                        QuantityPo = i.Quantity,
                        QuantityAccum = 0,
                        UnitPrice = i.UnitPrice,
                        DiscountPercent = i.DiscountPercent ?? purchaseOrderDetail.DiscountPercent,
                        AmountCurrent = 0,
                        AmountPo = i.Quantity * i.UnitPrice * ((1 - i.DiscountPercent) ?? (1 - purchaseOrderDetail.DiscountPercent)),
                        AmountAccum = 0
                    })
                    .ToList(),
                Violations = statement.Violations.Select(x => new StatementViolationDto
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Reason = x.Reason
                })
                .ToList()
            };

            foreach (var statementItemDto in statementDto.Items)
            {
                var statementItem = statement
                    .Items
                    .SingleOrDefault(x => x.PurchaseOrderItemId == statementItemDto.PurchaseOrderItemId);

                if (statementItem != null)
                {
                    statementItemDto.Id = statementItem.Id;
                    statementItemDto.QuantityCurrent = statementItem.Quantity;
                    statementItemDto.AmountCurrent = statementItem.Quantity * statementItemDto.UnitPrice * (1 - statementItemDto.DiscountPercent);
                }
            }

            var statementsIds = await _purchaseOrderStatementQuery
                .GetIdsUpToStatementNumberAsync(statement.PurchaseOrderId, statement.Number);

            var accumQuantitesListPerOrderItem = await _purchaseOrderStatementQuery
                .GetAccumQuantitiesPerOrderItemAsync(statementsIds);

            foreach (var statementItemDto in statementDto.Items)
            {
                var accumRow = accumQuantitesListPerOrderItem
                    .SingleOrDefault(x => x.PurchaseOrderItemId == statementItemDto.PurchaseOrderItemId);

                if (accumRow != null)
                {
                    statementItemDto.QuantityAccum = accumRow.TotalQuantity;
                    statementItemDto.AmountAccum = statementItemDto.QuantityAccum * statementItemDto.UnitPrice * (1 - statementItemDto.DiscountPercent);
                }
            }

            return statementDto;
        }

        public async Task ApproveQuantity(Guid statementId)
        {
            var statement = await _purchaseOrderStatementRepository.GetByIdAsync(statementId);
            if (statement == null)
                throw new NotFoundException("purchase order statement not found.");

            if (statement.State != PurchaseOrderStatementState.Created)
                throw new BusinessException("statement must be in created state before approve quantity");

            statement.State = PurchaseOrderStatementState.QuantityApproved;

            var storeId = await _purchaseOrderQuery.GetStoreIdByIdAsync(statement.PurchaseOrderId);

            if (storeId == null)
                throw new BusinessException("no store found");

            var forInventoryList = await _purchaseOrderStatementQuery.GetDetailsForInventoryAsync(statementId);

            foreach (var item in forInventoryList)
            {
                _inventoryCommandService.CreateInventoryMovement(new InventoryMovementCreationSnapshotDto
                {
                    StoreId = storeId.Value,
                    ItemId = item.ItemId,
                    UnitId = item.UnitId,
                    Quantity = item.Quantity,
                    TransactionId = item.StatementItemId
                });
            }

            await _uow.SaveChangesAsync();
        }

        public async Task Approve(Guid statementId)
        {
            var statement = await _purchaseOrderStatementRepository.GetByIdAsync(statementId);
            if (statement == null)
                throw new NotFoundException("purchase order statement not found.");

            if (statement.State != PurchaseOrderStatementState.QuantityApproved)
                throw new BusinessException("statement must be in Quantity approved state before approve");

            statement.State = PurchaseOrderStatementState.Approved;

            await _uow.SaveChangesAsync();
        }

        public async Task AddStatement(StatementCreationDto statementDto)
        {
            if (!(await ValidateStatementCreation(statementDto)))
                throw new BusinessException("statement infos are not valid");

            var statement = _mapper.Map<PurchaseOrderStatement>(statementDto);

            _purchaseOrderStatementRepository.Add(statement);
            await _uow.SaveChangesAsync();
        }

        public async Task UpdateStatement(Guid statementId, StatementUpdateDto statementDto)
        {
            var statement = await _purchaseOrderStatementRepository
                .GetByIdWithItemsAndViolationsAsync(statementId);

            if (statement == null)
                throw new NotFoundException("purchase order statement not found.");

            if (!(await ValidateStatementUpdate(statement, statementDto)))
                throw new BusinessException("statement infos are not valid");

            _mapper.Map(statementDto, statement);

            await _uow.SaveChangesAsync();
        }

        private async Task<bool> ValidateStatementCreation(StatementCreationDto statementDto)
        {
            var purchaseOrder = await _purchaseOrderQuery
                .GetByIdWithItemsAsync(statementDto.PurchaseOrderId);

            if (purchaseOrder == null || purchaseOrder.State != PurchaseOrderState.Approved)
                return false;

            decimal violationsAmount = statementDto.Violations.Sum(x => x.Amount);

            decimal statementAmount = 0;
            foreach (var statementItem in statementDto.Items)
            {
                var poItem = purchaseOrder.Items.Single(x => x.Id == statementItem.PurchaseOrderItemId);
                statementAmount += statementItem.Quantity * poItem.UnitPrice
                    * ((1 - poItem.DiscountPercent) ?? (1 - purchaseOrder.DiscountPercent));
            }

            if (statementAmount == 0 || statementAmount < violationsAmount)
                return false;

            var statementsIds = await _purchaseOrderStatementQuery.GetIdsAsync(purchaseOrder.Id);

            var accumQuantitesListPerOrderItem = await _purchaseOrderStatementQuery
                .GetAccumQuantitiesPerOrderItemAsync(statementsIds);

            foreach (var statementItem in statementDto.Items)
            {
                var poItem = purchaseOrder.Items.Single(x => x.Id == statementItem.PurchaseOrderItemId);

                var accumItem = accumQuantitesListPerOrderItem
                    .SingleOrDefault(x => x.PurchaseOrderItemId == statementItem.PurchaseOrderItemId);

                decimal PoQuantity = poItem.Quantity;
                decimal accumQuantity = 0;

                if (accumItem != null)
                    accumQuantity = accumItem.TotalQuantity;

                if (statementItem.Quantity + accumQuantity > PoQuantity)
                    return false;
            }

            return true;
        }

        private async Task<bool> ValidateStatementUpdate(PurchaseOrderStatement statement, StatementUpdateDto statementDto)
        {
            if (statement.State != PurchaseOrderStatementState.Created)
                return false;

            var purchaseOrder = await _purchaseOrderQuery
                .GetByIdWithItemsAsync(statement.PurchaseOrderId);

            if (purchaseOrder == null)
                return false;

            decimal violationsAmount = statementDto.Violations.Sum(x => x.Amount);

            decimal statementAmount = 0;
            foreach (var statementItem in statementDto.Items)
            {
                var poItem = purchaseOrder.Items.Single(x => x.Id == statementItem.PurchaseOrderItemId);
                statementAmount += statementItem.Quantity * poItem.UnitPrice
                    * ((1 - poItem.DiscountPercent) ?? (1 - purchaseOrder.DiscountPercent));
            }

            if (statementAmount == 0 || statementAmount < violationsAmount)
                return false;

            var statementsIds = await _purchaseOrderStatementQuery
                .GetIdsExceptAsync(purchaseOrder.Id, statement.Id);

            var accumQuantitesListPerOrderItem = await _purchaseOrderStatementQuery
                .GetAccumQuantitiesPerOrderItemAsync(statementsIds);

            foreach (var statementItem in statementDto.Items)
            {
                var poItem = purchaseOrder.Items.Single(x => x.Id == statementItem.PurchaseOrderItemId);

                var accumItem = accumQuantitesListPerOrderItem
                    .SingleOrDefault(x => x.PurchaseOrderItemId == statementItem.PurchaseOrderItemId);

                decimal PoQuantity = poItem.Quantity;
                decimal accumQuantity = 0;

                if (accumItem != null)
                    accumQuantity = accumItem.TotalQuantity;

                if (statementItem.Quantity + accumQuantity > PoQuantity)
                    return false;
            }

            return true;
        }
    }
}
