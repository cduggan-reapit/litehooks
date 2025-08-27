namespace Reapit.Platform.LiteHooks.Core.UseCases.Examples;

/// <summary>Validation failure messages used in ExampleEntity request validation.</summary>
internal static class ValidationMessages
{
    public const string Required = "Must not be empty.";
    public const string Unique = "Must be unique.";
    public const string NameExceedsMaxLength = "Exceeds maximum length of 100 characters";
    public const string DescriptionExceedsMaxLength = "Exceeds maximum length of 1,000 characters";
}