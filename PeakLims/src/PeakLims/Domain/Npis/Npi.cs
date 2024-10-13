namespace PeakLims.Domain.Npis;

using FluentValidation;

public sealed class NPI : ValueObject
{
    public string Value { get; private set; }
    
    public NPI(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Value = null;
            return;
        }
        new NPIValidator().ValidateAndThrow(value);
        Value = value;
    }
    
    public static NPI Of(string value) => new NPI(value);
    public static implicit operator string(NPI value) => value.Value;

    private NPI() { } // For EF Core or serialization

    private sealed class NPIValidator : AbstractValidator<string>
    {
        public NPIValidator()
        {
            RuleFor(npi => npi)
                .Must(BeAValidNPI)
                .WithMessage("Invalid NPI number.");
        }

        private bool BeAValidNPI(string npi)
        {
            // Rule 1: Must be exactly 10 digits long and numeric
            if (string.IsNullOrEmpty(npi) || npi.Length != 10 || !npi.All(char.IsDigit))
                return false;

            // Rule 2: Check digit validation using the Luhn algorithm adapted for NPI
            return IsValidCheckDigit(npi);
        }

        private bool IsValidCheckDigit(string npi)
        {
            // Step 1: Extract the first 9 digits (excluding the check digit)
            var baseNPI = npi[..9];

            // Step 2: Prepend the constant prefix "80840" to the base NPI
            var npiWithPrefix = "80840" + baseNPI;

            // Step 3: Apply the Luhn algorithm to the 13-digit number
            return Luhn(npiWithPrefix + npi[9]);
        }
        
        private static bool Luhn(string digits)
        {
            return digits.All(char.IsDigit) && digits.Reverse()
                .Select(c => c - '0')
                .Select((thisNum, i) => i % 2 == 0
                    ? thisNum
                    : ((thisNum *= 2) > 9 ? thisNum - 9 : thisNum)
                ).Sum() % 10 == 0;
        }
    }
}