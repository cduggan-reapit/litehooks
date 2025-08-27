using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.LiteHooks.Domain.Entities.Interfaces;
using Reapit.Platform.LiteHooks.Domain.Exceptions;
using System.Reflection;

namespace Reapit.Platform.LiteHooks.Domain.Entities.Abstract;

/// <summary>Base class from which service-owned entity types are derived.</summary>
public abstract class EntityBase : IHasCursor, IHasCreatedTimestamp, IHasModifiedTimestamp, ISerializableEntity
{
    /// <summary>The unique identifier of the entity.</summary>
    public string Id { get; }

    /// <inheritdoc />
    public long Cursor { get; }

    /// <summary>The creation timestamp for the entity.</summary>
    public DateTime DateCreated { get; }

    /// <summary>The timestamp of the last modification to the entity.</summary>
    public DateTime DateModified { get; private set; }

    /// <summary>The timestamp of the soft-deletion of the entity.</summary>
    public DateTime? DateDeleted { get; private set; }

    /// <summary>Flag indicating whether the entity has changed since instantiation.</summary>
    /// <remarks>This value should not be persisted.</remarks>
    public bool IsDirty { get; private set; }

    /// <summary>Initializes a new instance of the <see cref="EntityBase"/> class.</summary>
    protected EntityBase()
    {
        Id = IdentifierProvider.Generate;

        // Grab the timestamp. We keep this as DateTimeOffset so that we get microsecond resolution on the cursor.
        var timestamp = DateTimeOffsetProvider.Now;
        DateCreated = timestamp.UtcDateTime;
        DateModified = timestamp.UtcDateTime;
        Cursor = (long)(timestamp - DateTimeOffset.UnixEpoch).TotalMicroseconds;
    }

    /// <summary>Mark the entity as deleted.</summary>
    public void Delete()
    {
        DateModified = DateTimeOffsetProvider.Now.UtcDateTime;
        DateDeleted = DateModified;
        IsDirty = true;
    }

    /// <summary>Set the value of a property if it has changed.</summary>
    /// <param name="propertyName">The name of the property to update.</param>
    /// <param name="value">The type of the property to update.</param>
    /// <exception cref="Exception"></exception>
    internal void UpdateProperty(string propertyName, object? value)
    {
        // If value is null, we never change current
        if (value is null)
            return;

        // If the property is static, does not exist, cannot be read, or cannot be written, throw exception. 
        var property = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                           .FirstOrDefault(p =>
                               p.Name.Equals(propertyName, StringComparison.Ordinal) &&
                               p is { CanRead: true, CanWrite: true })
                       ?? throw EntityUpdateException.ForMissingProperty(GetType(), propertyName);

        var type = property.PropertyType;
        if (!value.GetType().IsAssignableTo(type))
            throw EntityUpdateException.ForIncorrectType(GetType(), propertyName, type, value.GetType());

        // If the value is unchanged, we don't need to try to modify it
        var currentValue = property.GetValue(this);
        if (value.Equals(currentValue))
            return;

        property.SetValue(this, value);
        DateModified = DateTimeOffsetProvider.Now.UtcDateTime;
        IsDirty = true;
    }
    
    public abstract object AsSerializable();
}