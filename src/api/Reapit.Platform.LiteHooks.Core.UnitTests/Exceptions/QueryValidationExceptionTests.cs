using FluentValidation.Results;
using Reapit.Platform.LiteHooks.Core.Exceptions;

namespace Reapit.Platform.LiteHooks.Core.UnitTests.Exceptions;

public static class QueryValidationExceptionTests
{
    public class Constructor
    {
        [Fact]
        public void Should_InitializeException_WithMessage()
        {
            const string message = nameof(Should_InitializeException_WithMessage);
            var sut = new QueryValidationException(message);
            Assert.Equal(message, sut.Message);
        }

        [Fact]
        public void Should_InitializeException_WithValidationFailures()
        {
            var errors = new[]
            {
                new ValidationFailure("property", "error"),
                new ValidationFailure("property", "error"),
                new ValidationFailure("property", "error")
            };

            var sut = new QueryValidationException(errors);
            sut.Message.Must().Be(QueryValidationException.DefaultMessage);
            sut.Errors.Must().HaveCount(3).And.BeEquivalentTo(errors);
        }
    }
}