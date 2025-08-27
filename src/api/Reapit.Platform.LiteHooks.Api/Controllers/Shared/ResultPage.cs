using System.Text.Json.Serialization;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Shared;

/// <summary>A page of <typeparamref name="TModel"/> objects.</summary>
/// <param name="Data">The collection of objects.</param>
/// <param name="Count">The number of objects included in this page.</param>
/// <param name="Cursor">The cursor to use when fetching the next page of results.</param>
/// <typeparam name="TModel">The type of model.</typeparam>
public record ResultPage<TModel>(
    [property: JsonPropertyName("data")] IEnumerable<TModel> Data,
    [property: JsonPropertyName("item_count")] int Count,
    [property: JsonPropertyName("next_cursor")] long Cursor)
    where TModel : class;