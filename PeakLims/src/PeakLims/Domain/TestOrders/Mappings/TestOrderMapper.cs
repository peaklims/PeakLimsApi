namespace PeakLims.Domain.TestOrders.Mappings;

using PeakLims.Domain.TestOrders.Dtos;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class TestOrderMapper
{
    [MapProperty(new[] { nameof(TestOrder.PanelOrder), nameof(TestOrder.PanelOrder.Id) }, new[] { nameof(TestOrderDto.PanelId) })]
    [MapProperty(new[] { nameof(TestOrder.Test), nameof(TestOrder.Test.Id) }, new[] { nameof(TestOrderDto.TestId) })]
    public static partial TestOrderDto ToTestOrderDto(this TestOrder testOrder);
    
    [MapProperty(new[] { nameof(TestOrder.PanelOrder), nameof(TestOrder.PanelOrder.Id) }, new[] { nameof(TestOrderDto.PanelId) })]
    [MapProperty(new[] { nameof(TestOrder.Test), nameof(TestOrder.Test.Id) }, new[] { nameof(TestOrderDto.TestId) })]
    public static partial IQueryable<TestOrderDto> ToTestOrderDtoQueryable(this IQueryable<TestOrder> testOrder);
}