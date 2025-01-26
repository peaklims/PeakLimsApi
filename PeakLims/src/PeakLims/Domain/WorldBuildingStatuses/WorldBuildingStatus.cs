namespace PeakLims.Domain.WorldBuildingStatuses;

using Ardalis.SmartEnum;
using PeakLims.Exceptions;

public sealed class WorldBuildingStatus : ValueObject
{
    private WorldBuildingStatusEnum _worldBuildingStatus;
    public string Value
    {
        get => _worldBuildingStatus.Name;
        private set
        {
            if (!WorldBuildingStatusEnum.TryFromName(value, true, out var parsed))
                throw new ValidationException($"Invalid World building status. Please use one of the following: {string.Join(", ", ListNames())}");

            _worldBuildingStatus = parsed;
        }
    }
    
    public WorldBuildingStatus(string value)
    {
        Value = value;
    }

    public static WorldBuildingStatus Of(string value) => new WorldBuildingStatus(value);
    public static implicit operator string(WorldBuildingStatus value) => value.Value;
    public static List<string> ListNames() => WorldBuildingStatusEnum.List.Select(x => x.Name).ToList();

   public static WorldBuildingStatus Pending() => new WorldBuildingStatus(WorldBuildingStatusEnum.Pending.Name);
   public static WorldBuildingStatus Processing() => new WorldBuildingStatus(WorldBuildingStatusEnum.Processing.Name);
   public static WorldBuildingStatus Successful() => new WorldBuildingStatus(WorldBuildingStatusEnum.Successful.Name);
   public static WorldBuildingStatus Failed() => new WorldBuildingStatus(WorldBuildingStatusEnum.Failed.Name);

    private WorldBuildingStatus() { } // EF Core

    private abstract class WorldBuildingStatusEnum(string name, int value)
        : SmartEnum<WorldBuildingStatusEnum>(name, value)
    {
        public static readonly WorldBuildingStatusEnum Pending = new PendingType();
        public static readonly WorldBuildingStatusEnum Processing = new ProcessingType();
        public static readonly WorldBuildingStatusEnum Successful = new SuccessfulType();
        public static readonly WorldBuildingStatusEnum Failed = new FailedType();

        private class PendingType() : WorldBuildingStatusEnum("Pending", 0);

        private class ProcessingType() : WorldBuildingStatusEnum("Processing", 1);

        private class SuccessfulType() : WorldBuildingStatusEnum("Successful", 2);

        private class FailedType() : WorldBuildingStatusEnum("Failed", 3);
    }
}