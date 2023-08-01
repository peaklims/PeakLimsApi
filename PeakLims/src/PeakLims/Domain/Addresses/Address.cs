namespace PeakLims.Domain.Addresses;

using SharedKernel.Domain;
using FluentValidation;

public class Address : ValueObject
{
    /// <summary>
    /// Address line 1 (e.g., street, PO Box, or company name).
    /// </summary>
    public string Line1 { get; }
    
    /// <summary>
    /// Address line 2 (e.g., apartment, suite, unit, or building).
    /// </summary>
    public string Line2 { get; }
    
    /// <summary>
    /// City, district, suburb, town, or village.
    /// </summary>
    public string City { get; }
    
    /// <summary>
    /// State, county, province, or region.
    /// </summary>
    public string State { get; }
    
    /// <summary>
    /// ZIP or postal code.
    /// </summary>
    public PostalCode PostalCode { get; }
    
    /// <summary>
    /// Two-letter country code <a href="https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2">(ISO 3166-1 alpha-2)</a>.
    /// </summary>
    public string Country { get; }
    
    public Address(string line1, string line2, string city, string state, string postalCode, string country)
        : this(line1, line2, city, state, PostalCode.Of(postalCode), country)
    {
    }

    public Address(string line1, string line2, string city, string state, PostalCode postalCode, string country)
    {
        // TODO country validation

        Line1 = line1;
        Line2 = line2;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
}

public class PostalCode : ValueObject
{
    public string Value { get; }
    public PostalCode(string value)
    {
        Value = value;
    }

    public static PostalCode Of(string postalCode) => new PostalCode(postalCode);
    public static implicit operator string(PostalCode postalCode) => postalCode.Value;
}