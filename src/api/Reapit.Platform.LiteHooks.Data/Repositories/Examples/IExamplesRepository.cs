using Reapit.Platform.LiteHooks.Data.Repositories.Common.Interfaces;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Data.Repositories.Examples;

/// <summary>
/// Describes a repository of <see cref="ExampleEntity"/> instances, which is capable of read and write operations.
/// </summary>
public interface IExamplesRepository : IReadOnlyExamplesRepository, IReadWriteRepository<ExampleEntity>
{
}