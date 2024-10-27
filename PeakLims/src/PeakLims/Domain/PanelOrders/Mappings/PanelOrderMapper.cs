namespace PeakLims.Domain.PanelOrders.Mappings;

using PeakLims.Domain.PanelOrders.Dtos;
using PeakLims.Domain.PanelOrders.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class PanelOrderMapper
{
    public static partial PanelOrderForUpdate ToPanelOrderForUpdate(this PanelOrderForUpdateDto panelOrderForUpdateDto);
    public static partial PanelOrderDto ToPanelOrderDto(this PanelOrder panelOrder);
    public static partial IQueryable<PanelOrderDto> ToPanelOrderDtoQueryable(this IQueryable<PanelOrder> panelOrder);
}