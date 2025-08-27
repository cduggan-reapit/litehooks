namespace Reapit.Platform.LiteHooks.Domain.Entities.Interfaces;

/// <summary>Interface implemented by entities which support cursor-based paging.</summary>
public interface IHasCursor
{
    /// <summary>The pagination cursor for this entity.</summary>
    public long Cursor { get; }
}