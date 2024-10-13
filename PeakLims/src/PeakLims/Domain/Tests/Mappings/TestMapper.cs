namespace PeakLims.Domain.Tests.Mappings;

using Panels.Dtos;
using PeakLims.Domain.Tests.Dtos;
using PeakLims.Domain.Tests.Models;
using Riok.Mapperly.Abstractions;
using TestOrders.Dtos;

[Mapper]
public static partial class TestMapper
{
    [MapperIgnoreTarget(nameof(TestForCreation.OrganizationId))]
    private static partial TestForCreation ToTestForCreation(this TestForCreationDto testForCreationDto);

    [MapperIgnoreTarget(nameof(TestForCreation.OrganizationId))]
    public static TestForCreation ToTestForCreation(this TestForCreationDto containerForCreationDto,
        Guid organizationId)
        => containerForCreationDto.ToTestForCreation()! with { OrganizationId = organizationId };
    
    public static partial TestForUpdate ToTestForUpdate(this TestForUpdateDto testForUpdateDto);
    public static partial TestDto ToTestDto(this Test test);
    public static partial IQueryable<TestDto> ToTestDtoQueryable(this IQueryable<Test> test);
    public static partial IQueryable<OrderablePanelsAndTestsDto.OrderableTest> ToOrderableTestQueryable(this IQueryable<Test> test);
}