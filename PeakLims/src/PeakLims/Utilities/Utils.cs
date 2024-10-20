namespace PeakLims.Utilities;

using System.Threading.Channels;

public static class Utils
{
    public static IAsyncEnumerable<V> MapParallel<U, V>(IEnumerable<U> source, Func<U, Task<V>> map)
    {
        var outputs = Channel.CreateUnbounded<V>();
        var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 15 };
        var mapTask = Parallel.ForEachAsync(source, parallelOptions, async (sourceItem, ct) =>
        {
            try
            {
                var mappedItem = await map(sourceItem);
                await outputs.Writer.WriteAsync(mappedItem, ct);
            }
            catch (Exception ex)
            {
                outputs.Writer.TryComplete(ex);
            }
        });

        mapTask.ContinueWith(_ => outputs.Writer.TryComplete());

        return outputs.Reader.ReadAllAsync();
    }

    public static async Task<TResult> RunWithRetries<TResult>(Func<Task<TResult>> operation, int backoffIncrement = 15)
    {
        var delay = TimeSpan.FromSeconds(5);
        var maxAttempts = 5;
        for (var attemptIndex = 1; ; attemptIndex++)
        {
            try
            {
                return await operation();
            }
            catch (Exception e) when (attemptIndex < maxAttempts)
            {
                Console.WriteLine($"Exception on attempt {attemptIndex}: {e.Message}. Will retry after {delay}");
                await Task.Delay(delay);
                delay += TimeSpan.FromSeconds(backoffIncrement);
            }
        }
    }

    public static TResult RunWithRetries<TResult>(Func<TResult> operation)
    {
        var maxAttempts = 5;
        for (var attemptIndex = 1; ; attemptIndex++)
        {
            try
            {
                return operation();
            }
            catch (Exception e) when (attemptIndex < maxAttempts)
            {
                Console.WriteLine($"Exception on attempt {attemptIndex}: {e.Message}. Retrying...");
            }
        }
    }

    // public static TResult RunWithRetries<TResult>(this Func<TResult> operation)
    // {
    //     var maxAttempts = 5;
    //     for (var attemptIndex = 1; ; attemptIndex++)
    //     {
    //         try
    //         {
    //             return operation();
    //         }
    //         catch (Exception e) when (attemptIndex < maxAttempts)
    //         {
    //         }
    //     }
    // }
}