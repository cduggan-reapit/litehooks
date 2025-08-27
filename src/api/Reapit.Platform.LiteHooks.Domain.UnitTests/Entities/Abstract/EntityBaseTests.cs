using Reapit.Platform.LiteHooks.Domain.Entities;
using Reapit.Platform.LiteHooks.Domain.Exceptions;

namespace Reapit.Platform.LiteHooks.Domain.UnitTests.Entities.Abstract;

public static class EntityBaseTests
{
    /*
     * Generally we wouldn't test an abstract class explicitly, but the update method is important enough and should
     * be ubiquitous enough to deserve dedicated testing.  We wouldn't want the exception happening in production,
     * so we shouldn't handle it's exceptions - they become 500's.
     */

    public class UpdateProperty
    {
        [Fact]
        public void Should_ThrowException_WhenPropertyNotFound()
        {
            const string propertyName = "doesn't exist";
            var expected = EntityUpdateException.ForMissingProperty(typeof(ExampleEntity), propertyName);

            var sut = CreateSut();
            var action = () => sut.UpdateProperty(propertyName, new { });
            action.Must().Throw<EntityUpdateException>().WithMessage(expected.Message);
        }

        [Fact]
        public void Should_ThrowException_WhenValueOfIncompatibleType()
        {
            var expected = EntityUpdateException.ForIncorrectType(typeof(ExampleEntity), "name", typeof(string), typeof(DateOnly));

            var sut = CreateSut();
            var action = () => sut.UpdateProperty(nameof(ExampleEntity.Name), DateOnly.MaxValue);
            action.Must().Throw<EntityUpdateException>().WithMessage(expected.Message);
        }
    }

    private static ExampleEntity CreateSut() => new ExampleEntity("name", "description");
}