using Validated.Primitives.Validation;

namespace Validated.Primitives.Core;

public abstract record ValidatedValueObject<T> where T : notnull
{
    // Exclude from record equality comparison by not making it a property
    [field: System.Runtime.CompilerServices.CompilerGenerated]
    protected readonly List<ValueValidator<T>> Validators = new();

    public T Value { get; }

    protected ValidatedValueObject(T value)
    {
        Value = value;
    }

    protected ValidatedValueObject(
        T value,
        params ValueValidator<T>[] validators)
    {
        Value = value;
        if (validators != null)
        {
            Validators.AddRange(validators);
        }
    }

    public ValidationResult Validate()
    {
        var aggregate = ValidationResult.Success();

        if (Validators.Count == 0)
            return aggregate;

        foreach (var validator in Validators)
        {
            if (validator is null) continue;
            var result = validator(Value);
            if (result is not null && !result.IsValid)
            {
                aggregate.Merge(result);
            }
        }

        return aggregate;
    }

    public override string ToString() => Value?.ToString() ?? string.Empty;

    // Override equality to only compare Value, excluding Validators
    public virtual bool Equals(ValidatedValueObject<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value!);
    }
}
