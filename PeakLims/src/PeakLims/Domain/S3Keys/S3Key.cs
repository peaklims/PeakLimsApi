namespace PeakLims.Domain.S3Keys;

using System.Text.RegularExpressions;
using Destructurama.Attributed;
using FluentValidation;

public sealed class S3Key : ValueObject
{
    [LogMasked]
    public string Value { get; set; }
    
    public S3Key(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = null;
            return;
        }
        new S3KeyValidator().ValidateAndThrow(value);
        Value = value;
    }
    
    public static S3Key Of(string value) => new S3Key(value);
    public static implicit operator string(S3Key value) => value.Value;

    private S3Key() { } // EF Core
    
    private sealed class S3KeyValidator : AbstractValidator<string> 
    {
        public S3KeyValidator()
        {
            RuleFor(x => x)
                .Must(HaveFileExtension)
                .WithMessage("The S3 key must end with a valid file extension.");
        }
    
        private bool HaveFileExtension(string value)
        {
            return Regex.IsMatch(value, @"\.\w{2,5}$");
        }
    }
}