using Validated.Primitives.Validation;

namespace Validated.Primitives.Core;

public abstract record ValidatedValueObject<T>: ValidatedPrimitive<T>
{
    protected ValidatedValueObject(T value) : base(value)
    {
    }

    protected ValidatedValueObject(T value, params ValueValidator<T>[] validators) : base(value, validators)
    {
    }
}