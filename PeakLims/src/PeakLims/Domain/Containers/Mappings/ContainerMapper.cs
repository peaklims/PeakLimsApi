namespace PeakLims.Domain.Containers.Mappings;

using PeakLims.Domain.Containers.Dtos;
using PeakLims.Domain.Containers.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class ContainerMapper
{
    [MapperIgnoreTarget(nameof(ContainerForCreation.OrganizationId))]
    private static partial ContainerForCreation ToContainerForCreation(this ContainerForCreationDto containerForCreationDto);

    [MapperIgnoreTarget(nameof(ContainerForCreation.OrganizationId))]
    public static ContainerForCreation ToContainerForCreation(this ContainerForCreationDto containerForCreationDto,
        Guid organizationId)
            => containerForCreationDto.ToContainerForCreation()! with { OrganizationId = organizationId };
    
    public static partial ContainerForUpdate ToContainerForUpdate(this ContainerForUpdateDto containerForUpdateDto);
    public static partial ContainerDto ToContainerDto(this Container container);
    public static partial IQueryable<ContainerDto> ToContainerDtoQueryable(this IQueryable<Container> container);
}