using System.Text.RegularExpressions;
using Validated.Primitives.Validation;
using Validated.Primitives.ValueObjects;

namespace Validated.Primitives.Validators;

public static partial class PhoneValidators
{
    public static ValueValidator<string> ValidFormat(string fieldName)
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var trimmed = value.Trim();

            if (!PhoneNumberRegex().IsMatch(trimmed))
            {
                return ValidationResult.Failure(
                    $"{fieldName} can only contain numbers, spaces, plus sign, hyphens, and parentheses.",
                    fieldName,
                    "InvalidPhoneNumberFormat");
            }

            return ValidationResult.Success();
        };

    public static ValueValidator<string> ValidateCountryFormat(string fieldName, CountryCode countryCode)
        => value =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return ValidationResult.Success();

            var normalized = value.Trim();
            var isValid = countryCode switch
            {
                CountryCode.UnitedStates => UsPhoneRegex().IsMatch(normalized),
                CountryCode.UnitedKingdom => UkPhoneRegex().IsMatch(normalized),
                CountryCode.Canada => CanadaPhoneRegex().IsMatch(normalized),
                CountryCode.Japan => JapanPhoneRegex().IsMatch(normalized),
                CountryCode.Netherlands => NetherlandsPhoneRegex().IsMatch(normalized),
                CountryCode.Germany => GermanyPhoneRegex().IsMatch(normalized),
                CountryCode.France => FrancePhoneRegex().IsMatch(normalized),
                CountryCode.Australia => AustraliaPhoneRegex().IsMatch(normalized),
                CountryCode.Spain => SpainPhoneRegex().IsMatch(normalized),
                CountryCode.Italy => ItalyPhoneRegex().IsMatch(normalized),
                CountryCode.Switzerland => SwitzerlandPhoneRegex().IsMatch(normalized),
                CountryCode.Austria => AustriaPhoneRegex().IsMatch(normalized),
                CountryCode.Belgium => BelgiumPhoneRegex().IsMatch(normalized),
                CountryCode.Sweden => SwedenPhoneRegex().IsMatch(normalized),
                CountryCode.Norway => NorwayPhoneRegex().IsMatch(normalized),
                CountryCode.Denmark => DenmarkPhoneRegex().IsMatch(normalized),
                CountryCode.Finland => FinlandPhoneRegex().IsMatch(normalized),
                CountryCode.Poland => PolandPhoneRegex().IsMatch(normalized),
                CountryCode.CzechRepublic => CzechPhoneRegex().IsMatch(normalized),
                CountryCode.Hungary => HungaryPhoneRegex().IsMatch(normalized),
                CountryCode.Portugal => PortugalPhoneRegex().IsMatch(normalized),
                CountryCode.Ireland => IrelandPhoneRegex().IsMatch(normalized),
                CountryCode.Brazil => BrazilPhoneRegex().IsMatch(normalized),
                CountryCode.Mexico => MexicoPhoneRegex().IsMatch(normalized),
                CountryCode.China => ChinaPhoneRegex().IsMatch(normalized),
                CountryCode.India => IndiaPhoneRegex().IsMatch(normalized),
                CountryCode.SouthAfrica => SouthAfricaPhoneRegex().IsMatch(normalized),
                CountryCode.NewZealand => NewZealandPhoneRegex().IsMatch(normalized),
                CountryCode.Singapore => SingaporePhoneRegex().IsMatch(normalized),
                CountryCode.SouthKorea => SouthKoreaPhoneRegex().IsMatch(normalized),
                CountryCode.Russia => RussiaPhoneRegex().IsMatch(normalized),
                _ => true
            };

            if (!isValid)
            {
                return ValidationResult.Failure(
                    $"{fieldName} is not a valid phone number format for {countryCode}.",
                    fieldName,
                    "InvalidCountryPhoneFormat");
            }

            return ValidationResult.Success();
        };

    // General phone number format: allows +, digits, spaces, hyphens, parentheses
    [GeneratedRegex(
        @"^[\+\d\s\-\(\)]+$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex PhoneNumberRegex();

    // United States: +1-XXX-XXX-XXXX or (XXX) XXX-XXXX or XXX-XXX-XXXX
    [GeneratedRegex(
        @"^(\+1[-\s]?)?(\(?\d{3}\)?[-\s]?)?\d{3}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex UsPhoneRegex();

    // United Kingdom: +44 XXXX XXXXXX or 0XXXX XXXXXX
    [GeneratedRegex(
        @"^(\+44\s?)?0?\d{4}\s?\d{6}$|^(\+44\s?)?0?\d{3}\s?\d{3}\s?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex UkPhoneRegex();

    // Canada: Same as US
    [GeneratedRegex(
        @"^(\+1[-\s]?)?(\(?\d{3}\)?[-\s]?)?\d{3}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex CanadaPhoneRegex();

    // Japan: +81-XX-XXXX-XXXX or 0XX-XXXX-XXXX
    [GeneratedRegex(
        @"^(\+81[-\s]?)?0?\d{1,4}[-\s]?\d{1,4}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex JapanPhoneRegex();

    // Netherlands: +31 XX XXX XXXX or 0XX XXX XXXX
    [GeneratedRegex(
        @"^(\+31[-\s]?)?0?\d{2}[-\s]?\d{3}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex NetherlandsPhoneRegex();

    // Germany: +49 XXX XXXXXXXX or 0XXX XXXXXXXX
    [GeneratedRegex(
        @"^(\+49[-\s]?)?0?\d{2,5}[-\s]?\d{3,10}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex GermanyPhoneRegex();

    // France: +33 X XX XX XX XX or 0X XX XX XX XX
    [GeneratedRegex(
        @"^(\+33[-\s]?)?0?\d{1}[-\s]?\d{2}[-\s]?\d{2}[-\s]?\d{2}[-\s]?\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex FrancePhoneRegex();

    // Australia: +61 X XXXX XXXX or 0X XXXX XXXX
    [GeneratedRegex(
        @"^(\+61[-\s]?)?0?\d{1}[-\s]?\d{4}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex AustraliaPhoneRegex();

    // Spain: +34 XXX XXX XXX
    [GeneratedRegex(
        @"^(\+34[-\s]?)?\d{3}[-\s]?\d{3}[-\s]?\d{3}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex SpainPhoneRegex();

    // Italy: +39 XXX XXXXXXX
    [GeneratedRegex(
        @"^(\+39[-\s]?)?\d{2,4}[-\s]?\d{6,8}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex ItalyPhoneRegex();

    // Switzerland: +41 XX XXX XX XX
    [GeneratedRegex(
        @"^(\+41[-\s]?)?0?\d{2}[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex SwitzerlandPhoneRegex();

    // Austria: +43 XXX XXXXXXX
    [GeneratedRegex(
        @"^(\+43[-\s]?)?0?\d{1,4}[-\s]?\d{3,10}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex AustriaPhoneRegex();

    // Belgium: +32 XXX XX XX XX
    [GeneratedRegex(
        @"^(\+32[-\s]?)?0?\d{3}[-\s]?\d{2}[-\s]?\d{2}[-\s]?\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex BelgiumPhoneRegex();

    // Sweden: +46 XX XXX XX XX
    [GeneratedRegex(
        @"^(\+46[-\s]?)?0?\d{2}[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex SwedenPhoneRegex();

    // Norway: +47 XXX XX XXX
    [GeneratedRegex(
        @"^(\+47[-\s]?)?\d{3}[-\s]?\d{2}[-\s]?\d{3}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex NorwayPhoneRegex();

    // Denmark: +45 XX XX XX XX
    [GeneratedRegex(
        @"^(\+45[-\s]?)?\d{2}[-\s]?\d{2}[-\s]?\d{2}[-\s]?\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex DenmarkPhoneRegex();

    // Finland: +358 XX XXX XX XX
    [GeneratedRegex(
        @"^(\+358[-\s]?)?0?\d{2}[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex FinlandPhoneRegex();

    // Poland: +48 XXX XXX XXX
    [GeneratedRegex(
        @"^(\+48[-\s]?)?\d{3}[-\s]?\d{3}[-\s]?\d{3}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex PolandPhoneRegex();

    // Czech Republic: +420 XXX XXX XXX
    [GeneratedRegex(
        @"^(\+420[-\s]?)?\d{3}[-\s]?\d{3}[-\s]?\d{3}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex CzechPhoneRegex();

    // Hungary: +36 XX XXX XXXX
    [GeneratedRegex(
        @"^(\+36[-\s]?)?0?\d{2}[-\s]?\d{3}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex HungaryPhoneRegex();

    // Portugal: +351 XXX XXX XXX
    [GeneratedRegex(
        @"^(\+351[-\s]?)?\d{3}[-\s]?\d{3}[-\s]?\d{3}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex PortugalPhoneRegex();

    // Ireland: +353 XX XXX XXXX
    [GeneratedRegex(
        @"^(\+353[-\s]?)?0?\d{2}[-\s]?\d{3}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex IrelandPhoneRegex();

    // Brazil: +55 XX XXXXX-XXXX or +55 XX XXXX-XXXX
    [GeneratedRegex(
        @"^(\+55[-\s]?)?\(?\d{2}\)?[-\s]?\d{4,5}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex BrazilPhoneRegex();

    // Mexico: +52 XXX XXX XXXX
    [GeneratedRegex(
        @"^(\+52[-\s]?)?\d{3}[-\s]?\d{3}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex MexicoPhoneRegex();

    // China: +86 XXX XXXX XXXX
    [GeneratedRegex(
        @"^(\+86[-\s]?)?\d{3}[-\s]?\d{4}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex ChinaPhoneRegex();

    // India: +91 XXXXX XXXXX
    [GeneratedRegex(
        @"^(\+91[-\s]?)?\d{5}[-\s]?\d{5}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex IndiaPhoneRegex();

    // South Africa: +27 XX XXX XXXX
    [GeneratedRegex(
        @"^(\+27[-\s]?)?0?\d{2}[-\s]?\d{3}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex SouthAfricaPhoneRegex();

    // New Zealand: +64 X XXX XXXX
    [GeneratedRegex(
        @"^(\+64[-\s]?)?0?\d{1}[-\s]?\d{3}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex NewZealandPhoneRegex();

    // Singapore: +65 XXXX XXXX
    [GeneratedRegex(
        @"^(\+65[-\s]?)?\d{4}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex SingaporePhoneRegex();

    // South Korea: +82 XX XXXX XXXX
    [GeneratedRegex(
        @"^(\+82[-\s]?)?0?\d{2}[-\s]?\d{4}[-\s]?\d{4}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex SouthKoreaPhoneRegex();

    // Russia: +7 XXX XXX-XX-XX
    [GeneratedRegex(
        @"^(\+7[-\s]?)?\d{3}[-\s]?\d{3}[-\s]?\d{2}[-\s]?\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
        matchTimeoutMilliseconds: 100)]
    private static partial Regex RussiaPhoneRegex();

    [Obsolete("Use ValidFormat and ValidateCountryFormat instead. This method will be removed in a future version.")]
    public static ValueValidator<string> PhoneNumber(string fieldName = "PhoneNumber")
        => value =>
        {
            return !PhoneNumberRegex().IsMatch(value ?? string.Empty)
                ? ValidationResult.Failure("Invalid phone number format.", fieldName, "PhoneNumber")
                : ValidationResult.Success();
        };
}
