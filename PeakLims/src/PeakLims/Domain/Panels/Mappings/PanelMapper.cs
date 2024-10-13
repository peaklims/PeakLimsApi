namespace PeakLims.Domain.Panels.Mappings;

using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Models;
using Riok.Mapperly.Abstractions;
using TestOrders.Dtos;

[Mapper]
public static partial class PanelMapper
{
    [MapperIgnoreTarget(nameof(PanelForCreation.OrganizationId))]
    private static partial PanelForCreation ToPanelForCreation(this PanelForCreationDto panelForCreationDto);

    [MapperIgnoreTarget(nameof(PanelForCreation.OrganizationId))]
    public static PanelForCreation ToPanelForCreation(this PanelForCreationDto containerForCreationDto,
        Guid organizationId)
        => containerForCreationDto.ToPanelForCreation()! with { OrganizationId = organizationId };
    
    public static partial PanelForUpdate ToPanelForUpdate(this PanelForUpdateDto panelForUpdateDto);
    public static partial PanelDto ToPanelDto(this Panel panel);
    public static partial IQueryable<PanelDto> ToPanelDtoQueryable(this IQueryable<Panel> panel);
    public static partial IQueryable<OrderablePanelsAndTestsDto.OrderablePanel> ToOrderablePanelQueryable(this IQueryable<Panel> panel);
}