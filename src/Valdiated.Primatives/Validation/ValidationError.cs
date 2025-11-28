namespace Validated.Primitives.Validation;

public sealed class ValidationError
{
    public string Message { get; }
    public string? MemberName { get; }
    public string? Code { get; }

    public ValidationError(string message, string? memberName = null, string? code = null)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        MemberName = memberName;
        Code = code;
    }

    public override string ToString()
        => string.IsNullOrEmpty(MemberName)
            ? Message
            : $"{MemberName}: {Message}";
}
