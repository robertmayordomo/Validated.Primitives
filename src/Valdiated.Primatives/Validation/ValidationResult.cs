namespace Validated.Primitives.Validation;

public sealed class ValidationResult
{
    private readonly List<ValidationError> _errors = new();

    public IReadOnlyList<ValidationError> Errors => _errors;
    public bool IsValid => _errors.Count == 0;

    private ValidationResult() { }

    public static ValidationResult Success()
        => new ValidationResult();

    public static ValidationResult Failure(string message, string? memberName = null, string? code = null)
    {
        var result = new ValidationResult();
        result.AddError(message, memberName, code);
        return result;
    }

    public void AddError(string message, string? memberName = null, string? code = null)
        => _errors.Add(new ValidationError(message, memberName, code));

    public ValidationResult Merge(ValidationResult other)
    {
        if (other == null) return this;
        _errors.AddRange(other.Errors);
        return this;
    }

    public string ToSingleMessage(string separator = "; ")
        => IsValid
            ? string.Empty
            : string.Join(separator, _errors.Select(e => e.ToString()));

    public string ToBulletList()
    {
        if (IsValid) return string.Empty;
        return string.Join(Environment.NewLine, _errors.Select(e => $" - {e}"));
    }

    public IDictionary<string, string[]> ToDictionary()
        => _errors
            .GroupBy(e => e.MemberName ?? string.Empty)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Message).ToArray());
}
