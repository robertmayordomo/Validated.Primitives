namespace Validated.Primitives.Validation;

public delegate ValidationResult ValueValidator<in T>(T value);
