namespace PeakLims.Utilities;

public static class IAsyncEnumerableExtensions
{
    public static async Task<List<T>> ConvertToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
    {
        var resultList = new List<T>();

        await foreach (var item in asyncEnumerable)
        {
            resultList.Add(item);
        }

        return resultList;
    }
}