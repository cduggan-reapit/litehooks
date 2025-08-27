using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Domain.UnitTests.Entities;

public static class ExampleEntityTests
{
    public class Constructor
    {
        [Fact]
        public void Should_SetIdentifier_AndCursor()
        {
            const string identifier = "dancing parrot";
            using var idFixture = new IdentifierProviderContext(identifier);

            var timestamp = DateTimeOffset.UtcNow;
            var expectedCursor = (long)(timestamp - DateTimeOffset.UnixEpoch).TotalMicroseconds;
            using var dtFixture = new DateTimeOffsetProviderContext(timestamp);

            var sut = CreateSut();
            sut.Id.Must().Be(identifier);
            sut.Cursor.Must().Be(expectedCursor);
        }
    }

    public class Update
    {
        [Fact]
        public void Should_NotChangeValues_WhenNullProvided()
        {
            var sut = CreateSut(); ;
            sut.Update(null, null);
            sut.IsDirty.Must().BeFalse();
            sut.DateModified.Must().Be(sut.DateCreated);
        }

        [Fact]
        public void Should_NotChangeValues_WhenValuesUnchanged()
        {
            var sut = CreateSut(); ;
            sut.Update(sut.Name, sut.Description);
            sut.IsDirty.Must().BeFalse();
            sut.DateModified.Must().Be(sut.DateCreated);
        }

        [Fact]
        public void Should_ChangeValues_WhenNewValuesProvided()
        {
            var sut = CreateSut();
            const string name = "updated name", description = "updated description";
            sut.Update(name, description);
            sut.IsDirty.Must().BeTrue();
            sut.DateModified.Must().NotBe(sut.DateCreated);

            sut.Must().Match<ExampleEntity>(entity => entity.Name.Equals(name) && description.Equals(entity.Description));
        }
    }

    public class Delete
    {
        [Fact]
        public void Should_SetDeletedTimestamp()
        {
            var sut = CreateSut();
            sut.SoftDelete();

            sut.DateDeleted.Must().NotBeNull();
        }
    }

    private static ExampleEntity CreateSut() => new("name", "description");
}