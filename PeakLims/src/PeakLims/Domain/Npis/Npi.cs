namespace PeakLims.Domain.Npis;

using FluentValidation;
using Utilities;

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
    public static NPI Of(int value) => new NPI(value.ToString());
    public static implicit operator string(NPI value) => value.Value;
    
    
    public static NPI Random()
    {
        return PeakLimsUtils.RunWithRetries(() =>
        {
            var random = new Random();
            // Generate a random 9-digit base NPI number
            var baseNpi = string.Concat(Enumerable.Range(0, 9).Select(_ => random.Next(0, 10)));
            // Calculate the check digit
            var checkDigit = CalculateCheckDigit(baseNpi);
            // Combine base NPI and check digit to form the full NPI
            var npi = baseNpi + checkDigit;
            return Of(npi);
        });
    }

    private NPI() { } // For EF Core or serialization

    /// <summary>
    /// Calculates the check digit for a given 9-digit base NPI using the Luhn algorithm adapted for NPI.
    /// </summary>
    private static int CalculateCheckDigit(string baseNpi)
    {
        // Prepend the constant prefix "80840" to the base NPI
        var npiWithPrefix = "80840" + baseNpi;

        // Convert the string into an array of digits
        var digits = npiWithPrefix.Select(c => c - '0').ToArray();

        // Apply the Luhn algorithm
        var sum = 0;
        var alternate = true; // Start from the rightmost digit
        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var temp = digits[i];
            if (alternate)
            {
                temp *= 2;
                if (temp > 9)
                    temp -= 9;
            }
            sum += temp;
            alternate = !alternate;
        }

        // Calculate the check digit
        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit;
    }

    private sealed class NPIValidator : AbstractValidator<string>
    {
        public NPIValidator()
        {
            RuleFor(npi => npi)
                .Must(BeAValidNPI)
                .WithMessage("Invalid NPI number '{PropertyValue}'.");
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