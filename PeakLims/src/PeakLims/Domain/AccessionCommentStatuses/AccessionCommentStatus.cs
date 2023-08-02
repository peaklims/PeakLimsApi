namespace PeakLims.Domain.AccessionCommentStatuses;

using Ardalis.SmartEnum;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

public class AccessionCommentStatus : ValueObject
{
    private AccessionCommentStatusEnum _status;
    public string Value
    {
        get => _status.Name;
        private set
        {
            if (!AccessionCommentStatusEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _status = parsed;
        }
    }
    
    public AccessionCommentStatus(string value)
    {
        Value = value;
    }
    public AccessionCommentStatus(AccessionCommentStatusEnum value)
    {
        Value = value.Name;
    }

    public bool IsActive() => Value == Active().Value;
    public bool IsArchived() => Value == Archived().Value;
    public static AccessionCommentStatus Of(string value) => new AccessionCommentStatus(value);
    public static implicit operator string(AccessionCommentStatus value) => value.Value;
    public static List<string> ListNames() => AccessionCommentStatusEnum.List.Select(x => x.Name).ToList();
    public static AccessionCommentStatus Active() => new AccessionCommentStatus(AccessionCommentStatusEnum.Active.Name);
    public static AccessionCommentStatus Archived() => new AccessionCommentStatus(AccessionCommentStatusEnum.Archived.Name);

    protected AccessionCommentStatus() { } // EF Core
}

public abstract class AccessionCommentStatusEnum : SmartEnum<AccessionCommentStatusEnum>
{
    public static readonly AccessionCommentStatusEnum Active = new ActiveType();
    public static readonly AccessionCommentStatusEnum Archived = new ArchivedType();

    protected AccessionCommentStatusEnum(string name, int value) : base(name, value)
    {
    }

    private class ActiveType : AccessionCommentStatusEnum
    {
        public ActiveType() : base("Active", 1)
        {
        }
    }

    private class ArchivedType : AccessionCommentStatusEnum
    {
        public ArchivedType() : base("Archived", 2)
        {
        }
    }
}