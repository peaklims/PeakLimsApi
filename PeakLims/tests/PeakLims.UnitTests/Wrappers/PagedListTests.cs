namespace PeakLims.UnitTests.Wrappers;

using PeakLims.Wrappers;
using FluentAssertions;
using Xunit;

public class PagedListTests
{
    [Fact]
    public void pagedlist_returns_accurate_data_for_standard_pagination()
    {
        var pageNumber = 2;
        var pageSize = 2;
        var source = new List<int>() { 1, 2, 3, 4, 5 }.AsQueryable();

        var list = PagedList<int>.Create(source, pageNumber, pageSize);
        list.TotalCount.Should().Be(5);
        list.PageSize.Should().Be(2);
        list.PageNumber.Should().Be(2);
        list.CurrentPageSize.Should().Be(2);
        list.CurrentStartIndex.Should().Be(3);
        list.CurrentEndIndex.Should().Be(4);
        list.TotalPages.Should().Be(3);
    }

    [Fact]
    public void pagedlist_returns_accurate_data_with_last_record()
    {
        var pageNumber = 3;
        var pageSize = 2;
        var source = new List<int>() { 1, 2, 3, 4, 5 }.AsQueryable();

        var list = PagedList<int>.Create(source, pageNumber, pageSize);
        list.TotalCount.Should().Be(5);
        list.PageSize.Should().Be(2);
        list.PageNumber.Should().Be(3);
        list.CurrentPageSize.Should().Be(1);
        list.CurrentStartIndex.Should().Be(5);
        list.CurrentEndIndex.Should().Be(5);
        list.TotalPages.Should().Be(3);
    }
}