namespace PeakLims.SharedTestHelpers.Fakes.Npi;

public static class NpiGenerator
{
    public static string GenerateRandomNpi()
    {
        var random = new Random();
        // Generate a random 9-digit base NPI number
        var baseNpi = string.Concat(Enumerable.Range(0, 9).Select(_ => random.Next(0, 10)));
        // Calculate the check digit
        var checkDigit = CalculateCheckDigit(baseNpi);
        // Combine base NPI and check digit to form the full NPI
        var npi = baseNpi + checkDigit;
        return npi;
    }

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
}