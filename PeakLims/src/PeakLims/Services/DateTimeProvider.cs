namespace PeakLims.Services;

public interface IDateTimeProvider
{
    DateTime DateTimeUtcNow { get; }
    DateOnly DateOnlyUtcNow { get; }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime DateTimeUtcNow => DateTime.UtcNow;
    public DateOnly DateOnlyUtcNow => DateOnly.FromDateTime(DateTimeUtcNow);
}