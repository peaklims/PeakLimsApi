namespace PeakLims.Domain.Panels.Mappings;

using PeakLims.Domain.Panels.Dtos;
using PeakLims.Domain.Panels.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class PanelMapper
{
    public static partial PanelForCreation ToPanelForCreation(this PanelForCreationDto panelForCreationDto);
    public static partial PanelForUpdate ToPanelForUpdate(this PanelForUpdateDto panelForUpdateDto);
    public static partial PanelDto ToPanelDto(this Panel panel);
    public static partial IQueryable<PanelDto> ToPanelDtoQueryable(this IQueryable<Panel> panel);
}