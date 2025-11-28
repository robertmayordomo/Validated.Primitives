using System.Text.RegularExpressions;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Validators;

public static partial class PostalCodeValidators
{
    public static ValueValidator<string> ValidFormat(string fieldName)
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var trimmed = value.Trim();

            if (!PostalCodeRegex().IsMatch(trimmed))
            {
                return ValidationResult.Failure(
                    $"{fieldName} can only contain letters, numbers, spaces, and hyphens.",
                    fieldName,
                    "InvalidPostalCodeFormat");
            }

            return ValidationResult.Success();
        };

    public static ValueValidator<string> ValidateCountryFormat(string fieldName, CountryCode countryCode)
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim().ToUpperInvariant();
            var isValid = countryCode switch
            {
                CountryCode.UnitedStates => UsPostalCodeRegex().IsMatch(normalized),
                CountryCode.UnitedKingdom => UkPostalCodeRegex().IsMatch(normalized),
                CountryCode.Canada => CanadaPostalCodeRegex().IsMatch(normalized),
                CountryCode.Japan => JapanPostalCodeRegex().IsMatch(normalized),
                CountryCode.Netherlands => NetherlandsPostalCodeRegex().IsMatch(normalized),
                CountryCode.Germany => GermanyPostalCodeRegex().IsMatch(normalized),
                CountryCode.France => FrancePostalCodeRegex().IsMatch(normalized),
                CountryCode.Australia => AustraliaPostalCodeRegex().IsMatch(normalized),
                CountryCode.Spain => SpainPostalCodeRegex().IsMatch(normalized),
                CountryCode.Italy => ItalyPostalCodeRegex().IsMatch(normalized),
                CountryCode.Switzerland => SwitzerlandPostalCodeRegex().IsMatch(normalized),
                CountryCode.Austria => AustriaPostalCodeRegex().IsMatch(normalized),
                CountryCode.Belgium => BelgiumPostalCodeRegex().IsMatch(normalized),
                CountryCode.Sweden => SwedenPostalCodeRegex().IsMatch(normalized),
                CountryCode.Norway => NorwayPostalCodeRegex().IsMatch(normalized),
                CountryCode.Denmark => DenmarkPostalCodeRegex().IsMatch(normalized),
                CountryCode.Finland => FinlandPostalCodeRegex().IsMatch(normalized),
                CountryCode.Poland => PolandPostalCodeRegex().IsMatch(normalized),
                CountryCode.CzechRepublic => CzechPostalCodeRegex().IsMatch(normalized),
                CountryCode.Hungary => HungaryPostalCodeRegex().IsMatch(normalized),
                CountryCode.Portugal => PortugalPostalCodeRegex().IsMatch(normalized),
                CountryCode.Ireland => IrelandPostalCodeRegex().IsMatch(normalized),
                CountryCode.Brazil => BrazilPostalCodeRegex().IsMatch(normalized),
                CountryCode.Mexico => MexicoPostalCodeRegex().IsMatch(normalized),
                CountryCode.China => ChinaPostalCodeRegex().IsMatch(normalized),
                CountryCode.India => IndiaPostalCodeRegex().IsMatch(normalized),
                CountryCode.SouthAfrica => SouthAfricaPostalCodeRegex().IsMatch(normalized),
                CountryCode.NewZealand => NewZealandPostalCodeRegex().IsMatch(normalized),
                CountryCode.Singapore => SingaporePostalCodeRegex().IsMatch(normalized),
                CountryCode.SouthKorea => SouthKoreaPostalCodeRegex().IsMatch(normalized),
                CountryCode.Russia => RussiaPostalCodeRegex().IsMatch(normalized),
                _ => true
            };

            if (!isValid)
            {
                return ValidationResult.Failure(
                    $"{fieldName} is not a valid postal code format for {countryCode}.",
                    fieldName,
                    "InvalidCountryPostalCodeFormat");
            }

            return ValidationResult.Success();
        };

    [GeneratedRegex(@"^[A-Za-z0-9\s\-]+$")]
    private static partial Regex PostalCodeRegex();

    [GeneratedRegex(@"^\d{5}(-\d{4})?$")]
    private static partial Regex UsPostalCodeRegex();

    [GeneratedRegex(@"^[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2}$")]
    private static partial Regex UkPostalCodeRegex();

    [GeneratedRegex(@"^[A-Z]\d[A-Z]\s?\d[A-Z]\d$")]
    private static partial Regex CanadaPostalCodeRegex();

    [GeneratedRegex(@"^\d{3}-?\d{4}$")]
    private static partial Regex JapanPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}\s?[A-Z]{2}$")]
    private static partial Regex NetherlandsPostalCodeRegex();

    [GeneratedRegex(@"^\d{5}$")]
    private static partial Regex GermanyPostalCodeRegex();

    [GeneratedRegex(@"^\d{5}$")]
    private static partial Regex FrancePostalCodeRegex();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex AustraliaPostalCodeRegex();

    [GeneratedRegex(@"^\d{5}$")]
    private static partial Regex SpainPostalCodeRegex();

    [GeneratedRegex(@"^\d{5}$")]
    private static partial Regex ItalyPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex SwitzerlandPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex AustriaPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex BelgiumPostalCodeRegex();

    [GeneratedRegex(@"^\d{3}\s?\d{2}$")]
    private static partial Regex SwedenPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex NorwayPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex DenmarkPostalCodeRegex();

    [GeneratedRegex(@"^\d{5}$")]
    private static partial Regex FinlandPostalCodeRegex();

    [GeneratedRegex(@"^\d{2}-\d{3}$")]
    private static partial Regex PolandPostalCodeRegex();

    [GeneratedRegex(@"^\d{3}\s\d{2}$")]
    private static partial Regex CzechPostalCodeRegex();

    [GeneratedRegex(@"^(H-)?\d{4}$")]
    private static partial Regex HungaryPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}-\d{3}$")]
    private static partial Regex PortugalPostalCodeRegex();

    [GeneratedRegex(@"^[A-Z]\d{2}\s?[A-Z0-9]{4}$")]
    private static partial Regex IrelandPostalCodeRegex();

    [GeneratedRegex(@"^\d{5}-?\d{3}$")]
    private static partial Regex BrazilPostalCodeRegex();

    [GeneratedRegex(@"^\d{5}$")]
    private static partial Regex MexicoPostalCodeRegex();

    [GeneratedRegex(@"^\d{6}$")]
    private static partial Regex ChinaPostalCodeRegex();

    [GeneratedRegex(@"^\d{6}$")]
    private static partial Regex IndiaPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex SouthAfricaPostalCodeRegex();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex NewZealandPostalCodeRegex();

    [GeneratedRegex(@"^\d{6}$")]
    private static partial Regex SingaporePostalCodeRegex();

    [GeneratedRegex(@"^\d{5}$")]
    private static partial Regex SouthKoreaPostalCodeRegex();

    [GeneratedRegex(@"^\d{6}$")]
    private static partial Regex RussiaPostalCodeRegex();
}
