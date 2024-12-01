namespace PeakLims.Utilities;

public static class DateOnlyExtensions
{
    public static DateOnly Today()
    {
        return DateOnly.FromDateTime(DateTimeOffset.UtcNow.Date);
    }
}
