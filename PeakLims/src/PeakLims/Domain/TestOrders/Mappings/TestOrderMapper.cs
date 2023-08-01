namespace PeakLims.Domain.TestOrders.Mappings;

using PeakLims.Domain.TestOrders.Dtos;
using PeakLims.Domain.TestOrders.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class TestOrderMapper
{
    public static partial TestOrderForCreation ToTestOrderForCreation(this TestOrderForCreationDto testOrderForCreationDto);
    public static partial TestOrderForUpdate ToTestOrderForUpdate(this TestOrderForUpdateDto testOrderForUpdateDto);
    public static partial TestOrderDto ToTestOrderDto(this TestOrder testOrder);
    public static partial IQueryable<TestOrderDto> ToTestOrderDtoQueryable(this IQueryable<TestOrder> testOrder);
}