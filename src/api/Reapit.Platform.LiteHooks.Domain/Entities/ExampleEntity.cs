using Reapit.Platform.LiteHooks.Domain.Entities.Abstract;

namespace Reapit.Platform.LiteHooks.Domain.Entities;

/// <summary>Example entity provided with the template project.</summary>
/// <param name="name">The name of the entity.</param>
/// <param name="description">A description of the entity.</param>
public class ExampleEntity(string name, string? description) : EntityBase
{
    /// <summary>The entity name.</summary>
    public string Name { get; private set; } = name;

    /// <summary>
    /// The entity description.
    /// </summary>
    public string? Description { get; private set; } = description;

    /// <summary>Update the properties of the entity.</summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="description">The description of the property.</param>
    /// <remarks>
    /// Does not modify properties if the provided value is null or is equal to the current value. If changes ar applied,
    /// <see cref="EntityBase.IsDirty"/> will be true and <see cref="EntityBase.DateModified"/> will be updated.
    /// </remarks>
    public void Update(string? name, string? description)
    {
        UpdateProperty(nameof(Name), name);
        UpdateProperty(nameof(Description), description);
    }

    public override object AsSerializable() 
        => new { Id, Created = DateCreated, Modified = DateModified, IsDeleted = DateDeleted is not null, Name };
}