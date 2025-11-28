namespace Validated.Primitives.Validation;

public sealed class ValueObjectValidationException : Exception
{
    public Type ValueObjectType { get; }
    public ValidationResult ValidationResult { get; }

    public ValueObjectValidationException(Type valueObjectType, ValidationResult validationResult)
        : base(validationResult.ToSingleMessage())
    {
        ValueObjectType = valueObjectType;
        ValidationResult = validationResult;
    }
}
