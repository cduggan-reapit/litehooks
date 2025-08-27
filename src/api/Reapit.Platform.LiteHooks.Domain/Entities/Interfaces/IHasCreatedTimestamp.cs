namespace Reapit.Platform.LiteHooks.Domain.Entities.Interfaces;

/// <summary>Interface implemented by entities which include a creation timestamp.</summary>
public interface IHasCreatedTimestamp
{
    /// <summary>The creation timestamp for the entity.</summary>
    public DateTime DateCreated { get; }
}