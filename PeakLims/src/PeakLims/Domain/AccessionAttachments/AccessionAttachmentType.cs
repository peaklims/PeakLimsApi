namespace PeakLims.Domain.AccessionAttachments;

using Ardalis.SmartEnum;
using Exceptions;

public sealed class AccessionAttachmentType : ValueObject
{
    private AccessionAttachmentTypeEnum _type;
    public string Value
    {
        get => _type?.Name ?? null;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _type = null;
                return;
            }
            
            if (!AccessionAttachmentTypeEnum.TryFromName(value, true, out var parsed))
                throw new InvalidSmartEnumPropertyName(nameof(Value), value);

            _type = parsed;
        }
    }
    
    public AccessionAttachmentType(string value)
    {
        Value = value;
    }
    
    public static AccessionAttachmentType Of(string value) => new AccessionAttachmentType(value);
    public static implicit operator string(AccessionAttachmentType value) => value.Value;
    public static List<string> ListNames() => AccessionAttachmentTypeEnum.List.Select(x => x.Name).ToList();

    public static AccessionAttachmentType TestRequisition() => new AccessionAttachmentType(AccessionAttachmentTypeEnum.TestRequisition.Name);
    public static AccessionAttachmentType LabReport() => new AccessionAttachmentType(AccessionAttachmentTypeEnum.LabReport.Name);
    public static AccessionAttachmentType Other() => new AccessionAttachmentType(AccessionAttachmentTypeEnum.Other.Name);

    private AccessionAttachmentType() { } // EF Core
    
    private class AccessionAttachmentTypeEnum : SmartEnum<AccessionAttachmentTypeEnum>
    {
        public static readonly AccessionAttachmentTypeEnum TestRequisition = new TestRequisitionType();
        public static readonly AccessionAttachmentTypeEnum LabReport = new LabReportType();
        public static readonly AccessionAttachmentTypeEnum Other = new OtherType();

        protected AccessionAttachmentTypeEnum(string name, int value) : base(name, value)
        {
        }

        private class TestRequisitionType : AccessionAttachmentTypeEnum
        {
            public TestRequisitionType() : base("Test Requisition", 0)
            {
            }
        }

        private class LabReportType : AccessionAttachmentTypeEnum
        {
            public LabReportType() : base("Lab Report", 1)
            {
            }
        }

        private class OtherType : AccessionAttachmentTypeEnum
        {
            public OtherType() : base("Other", 2)
            {
            }
        }
    }
}