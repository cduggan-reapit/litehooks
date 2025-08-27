namespace Reapit.Platform.LiteHooks.Domain.Exceptions;

/// <summary>Represents an error arising when attempting to update the properties of an entity.</summary>
public class EntityUpdateException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="EntityUpdateException"/> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    private EntityUpdateException(string message) : base(message)
    {
    }

    /// <summary>Represents an error arising from attempting to write to a non-existent or inaccessible property.</summary>
    /// <param name="parentType">The type containing the property to which the user attempted to write.</param>
    /// <param name="propertyName">The name of the property to which the user attempted to write.</param>
    public static EntityUpdateException ForMissingProperty(Type parentType, string propertyName)
        => new($"Property not found or inaccessible: '{parentType.Name}.{propertyName}'.");

    /// <summary>Represents an error arising from attempting to assign an invalid value to a property.</summary>
    /// <param name="parentType">The type containing the property to which the user attempted to write.</param>
    /// <param name="propertyName">The name of the property to which the user attempted to write.</param>
    /// <param name="expectedType">The type of the property.</param>
    /// <param name="actualType">The type of the value that the user attempted to assign to the property.</param>
    public static EntityUpdateException ForIncorrectType(Type parentType, string propertyName, Type expectedType, Type actualType)
        => new($"Cannot assign {actualType.Name} to {expectedType.Name} member: '{parentType.Name}.{propertyName}'.");
}