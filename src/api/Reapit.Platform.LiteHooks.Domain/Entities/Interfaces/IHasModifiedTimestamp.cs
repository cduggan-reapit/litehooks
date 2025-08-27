namespace Reapit.Platform.LiteHooks.Domain.Entities.Interfaces;

/// <summary>Interface implemented by entities which include a last modification timestamp.</summary>
public interface IHasModifiedTimestamp
{
    /// <summary>The timestamp of the last modification to the entity.</summary>
    public DateTime DateModified { get; }
}