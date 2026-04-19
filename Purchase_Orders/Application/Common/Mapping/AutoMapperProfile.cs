using AutoMapper;
using Purchase_Orders.Domain.PurchaseOrders;
using Purchase_Orders.Domain.PurchaseOrders.Enums;
using Purchase_Orders.Domain.Setup;
using Purchase_Orders.Application.Dtos.PurchaseOrders;
using Purchase_Orders.Application.Dtos.PurchaseOrderStatements;

namespace Purchase_Orders.Application.Common.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PurchaseOrderItemCreationDto, PurchaseOrderItem>();
            CreateMap<PurchaseOrderCreationDto, PurchaseOrder>()
                .ForMember(dest => dest.State,
                opt => opt.MapFrom(src => PurchaseOrderState.Created))
                
                .ForMember(dest => dest.Date,
               opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Parse(src.Date))));

            CreateMap<PurchaseOrderItemUpdateDto, PurchaseOrderItem>();
            CreateMap<PurchaseOrderUpdateDto, PurchaseOrder>()
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Parse(src.Date))))
                .ForMember(x => x.Items, m => m.Ignore())
                .AfterMap(MapPurchaseOrderItems);

            CreateMap<PurchaseOrderItem, PurchaseOrderItemUpdateGetDto>()
                .ForMember(
                    d => d.DiscountPercent,
                    m => m.MapFrom(s =>
                        s.DiscountPercent.HasValue
                            ? (decimal?)(s.DiscountPercent.Value * 100)
                            : null
                    )
                );
            CreateMap<PurchaseOrder, PurchaseOrderUpdateGetDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd")))
                .ForMember(x => x.DiscountPercent, m => m.MapFrom(x => x.DiscountPercent * 100))
                .ForMember(x => x.RetentionPercent, m => m.MapFrom(x => x.RetentionPercent * 100))
                .ForMember(x => x.State, m => m.MapFrom(x => (short)x.State));

            CreateMap<StatementItemUpdateDto, PurchaseOrderStatementItem>();
            CreateMap<StatementViolationUpdateDto, PurchaseOrderViolationDiscount>();
            CreateMap<StatementUpdateDto, PurchaseOrderStatement>()
                .ForMember(x => x.Items, m => m.Ignore())
                .ForMember(x => x.Violations, m => m.Ignore())
                .AfterMap(MapStatementItems)
                .AfterMap(MapViolations);


            CreateMap<StatementItemCreationDto, PurchaseOrderStatementItem>();
            CreateMap<StatementViolationCreationDto, PurchaseOrderViolationDiscount>();
            CreateMap<StatementCreationDto, PurchaseOrderStatement>()
                .ForMember(x => x.State, m => m.MapFrom(x => PurchaseOrderStatementState.Created));
        }

        private void MapPurchaseOrderItems(PurchaseOrderUpdateDto PurchaseOrderUpdateDto, PurchaseOrder PurchaseOrder)
        {
            foreach (var item in PurchaseOrderUpdateDto.Items)
            {
                if (!item.Id.HasValue)
                {
                    PurchaseOrder.Items.Add(new PurchaseOrderItem 
                    {
                        LineNo = item.LineNo,
                        ItemId = item.ItemId, 
                        UnitId = item.UnitId, 
                        Quantity = item.Quantity, 
                        UnitPrice = item.UnitPrice,
                        DiscountPercent = item.DiscountPercent,
                        PurchaseOrderId = PurchaseOrder.Id 
                    });
                }
                else
                {
                    var pIItem = PurchaseOrder.Items.SingleOrDefault(x => x.Id == item.Id);
                    //Update existing item
                    if (pIItem != null)
                    {
                        pIItem.LineNo = item.LineNo;
                        pIItem.ItemId = item.ItemId;
                        pIItem.UnitId = item.UnitId;
                        pIItem.Quantity = item.Quantity;
                        pIItem.UnitPrice = item.UnitPrice;
                        pIItem.DiscountPercent = item.DiscountPercent;
                    }
                }
            }

            //Get list of invoice item ids that don't exist in dto
            var PurchaseOrderItemsToDeleteIds = PurchaseOrder.Items.Where(x => !PurchaseOrderUpdateDto.Items.Select(p => p.Id).Contains(x.Id)).Select(x => x.Id).ToList();

            foreach (var id in PurchaseOrderItemsToDeleteIds)
            {
                if (id != System.Guid.Empty)
                    PurchaseOrder.Items.Remove(PurchaseOrder.Items.First(x => x.Id == id));
            }
        }

        private void MapStatementItems(StatementUpdateDto statementUpdateDto, PurchaseOrderStatement statement)
        {
            foreach (var item in statementUpdateDto.Items)
            {
                if (!item.Id.HasValue)
                {
                    //Add new item
                    statement.Items.Add(new PurchaseOrderStatementItem
                    {
                        LineNo = item.LineNo,
                        Quantity = item.Quantity,
                        PurchaseOrderItemId = item.PurchaseOrderItemId
                    });
                }
                else
                {
                    var statementItem = statement.Items.SingleOrDefault(x => x.Id == item.Id);
                    //Update existing item
                    if (statementItem != null)
                    {
                        statementItem.LineNo = item.LineNo;
                        statementItem.Quantity = item.Quantity;
                        statementItem.PurchaseOrderItemId = item.PurchaseOrderItemId;
                    }
                }
            }

            //Get list of Statement item ids that don't exist in dto
            var statementItemsToDeleteIds = statement.Items.Where(x => !statementUpdateDto.Items.Select(p => p.Id).Contains(x.Id)).Select(x => x.Id).ToList();

            foreach (var id in statementItemsToDeleteIds)
            {
                if (id != System.Guid.Empty)
                    statement.Items.Remove(statement.Items.First(x => x.Id == id));
            }
        }

        private void MapViolations(StatementUpdateDto statementUpdateDto, PurchaseOrderStatement statement)
        {
            foreach (var item in statementUpdateDto.Violations)
            {
                if (!item.Id.HasValue)
                {
                    //Add new item
                    statement.Violations.Add(new PurchaseOrderViolationDiscount
                    {
                        Amount = item.Amount,
                        Reason = item.Reason
                    });
                }
                else
                {
                    var statementViolationItem = statement.Violations.SingleOrDefault(x => x.Id == item.Id);
                    //Update existing item
                    if (statementViolationItem != null)
                    {
                        statementViolationItem.Amount = item.Amount;
                        statementViolationItem.Reason = item.Reason;
                    }
                }
            }

            //Get list of Statement Violation item ids that don't exist in dto
            var statementViolationItemsToDeleteIds = statement.Violations.Where(x => !statementUpdateDto.Violations.Select(p => p.Id).Contains(x.Id)).Select(x => x.Id).ToList();

            foreach (var id in statementViolationItemsToDeleteIds)
            {
                //Remove deleted item
                if (id != System.Guid.Empty)
                    statement.Violations.Remove(statement.Violations.First(x => x.Id == id));
            }
        }
    }
}
