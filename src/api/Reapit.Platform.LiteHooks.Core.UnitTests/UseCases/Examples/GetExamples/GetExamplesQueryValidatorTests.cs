using Reapit.Platform.LiteHooks.Core.UseCases;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.UseCases.Examples.GetExamples;

public static class GetExamplesQueryValidatorTests
{
    public class ValidateAsync
    {
        [Fact]
        public async Task Should_Pass_WhenQueryValid()
        {
            var query = GetRequest();
            var sut = CreateSut();
            var result = await sut.ValidateAsync(query);
            result.IsValid.Must().BeTrue();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(101)]
        public async Task Should_Fail_WhenPageSizeInvalid(int pageSize)
        {
            var query = GetRequest(pageSize);
            var sut = CreateSut();
            var result = await sut.ValidateAsync(query);

            result.Errors.Must().HaveCount(1).And.Contain(
                failure => failure.PropertyName == nameof(GetExamplesQuery.PageSize)
                           && failure.ErrorMessage == GenericValidationMessages.PageSizeOutOfRange);
        }
    }

    private static GetExamplesQueryValidator CreateSut() => new();

    private static GetExamplesQuery GetRequest(int pageSize = 25)
        => new(PageSize: pageSize);
}