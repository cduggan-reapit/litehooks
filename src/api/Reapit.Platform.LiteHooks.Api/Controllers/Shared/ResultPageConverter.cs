using AutoMapper;
using Reapit.Platform.LiteHooks.Domain.Entities.Interfaces;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Shared;

/// <summary>Converts a `TEntity` collection to a result page containing `TModel` objects.</summary>
/// <typeparam name="TEntity">The type of entity, must implement <see cref="IHasCursor"/>.</typeparam>
/// <typeparam name="TModel">The type of model to convert `TEntity` instances to.</typeparam>
/// <remarks>
/// The AutoMapper profile using this converter must contain a mapping definition from
/// <typeparamref name="TEntity"/> to <typeparamref name="TModel"/>.
/// </remarks>
public class ResultPageConverter<TEntity, TModel>
    : ITypeConverter<IEnumerable<TEntity>, ResultPage<TModel>>
    where TEntity : IHasCursor
    where TModel : class
{
    /// <summary>Performs conversion from a source collection to a result page.</summary>
    /// <param name="source">The source collection.</param>
    /// <param name="destination">The destination object.</param>
    /// <param name="context">The AutoMapper resolution context.</param>
    /// <returns>The destination object.</returns>
    public ResultPage<TModel> Convert(
        IEnumerable<TEntity> source,
        ResultPage<TModel> destination,
        ResolutionContext context)
    {
        // Enumerate the collection by calling ToList.
        var list = source.ToList();

        // Perform the conversion.
        destination = new ResultPage<TModel>(
            Data: context.Mapper.Map<IEnumerable<TModel>>(list),
            Count: list.Count,
            Cursor: GetMaximumCursor(list));

        // Return the generated model.
        return destination;
    }

    private static long GetMaximumCursor(ICollection<TEntity> list)
        => list.Any()
            ? list.Max(item => item.Cursor)
            : 0;
}