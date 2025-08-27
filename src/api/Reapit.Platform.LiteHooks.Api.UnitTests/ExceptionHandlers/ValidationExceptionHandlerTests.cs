using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.Testing.Extensions;
using Reapit.Platform.LiteHooks.Api.ExceptionHandlers;

namespace Reapit.Platform.LiteHooks.Api.UnitTests.ExceptionHandlers;

public static class ValidationExceptionHandlerTests
{
    public class TryHandleAsync
    {
        [Fact]
        public async Task TryHandleAsync_ReturnsFalse_WhenExceptionNotValidationException()
        {
            var exception = new InvalidOperationException("that was an invalid operation!");

            var sut = CreateSut();
            var actual = await sut.TryHandleAsync(new DefaultHttpContext(), exception, CancellationToken.None);
            actual.Must().BeFalse();
        }

        [Fact]
        public async Task TryHandleAsync_HandlesValidationException_AndReturnsTrue()
        {
            var exception = new ValidationException("That was an exceptional argument!");

            var expectedStatus = ValidationExceptionHandler.StatusCode;
            var expectedContent = ValidationExceptionHandler.GetProblemDetails(exception);

            using var responseStream = new MemoryStream();
            var context = new DefaultHttpContext { Response = { Body = responseStream } };

            var sut = CreateSut();
            var actual = await sut.TryHandleAsync(context, exception, CancellationToken.None);

            actual.Must().BeTrue();

            context.Response.StatusCode.Must().Be(expectedStatus);
            (await responseStream.RewindAndReadAsJsonAsync<ProblemDetails>()).Must().BeEquivalentTo(expectedContent);
        }
    }

    public class GetProblemDetails
    {
        [Fact]
        public void GetProblemDetails_ReturnsProblemDetails_WithoutExtensions_WhenNoErrorsPresent()
        {
            var input = new ValidationException("because");
            var actual = ValidationExceptionHandler.GetProblemDetails(input);
            actual.Extensions.Must().NotContainKey("errors");
        }

        [Fact]
        public void GetProblemDetails_ReturnsProblemDetails_WithExtensions_WhenErrorsPresent()
        {
            var validation = new ValidationResult([
                new ValidationFailure("property-1", "error-1"),
                new ValidationFailure("property-1", "error-2"),
                new ValidationFailure("property-1", "error-3"),
                new ValidationFailure("property-2", "error-1"),
                new ValidationFailure("property-2", "error-2"),
                new ValidationFailure("property-3", "error-1")
            ]);

            var input = new ValidationException(validation.Errors);

            var expectedDictionary = new Dictionary<string, IEnumerable<string>>
            {
                { "property-1", ["error-1", "error-2", "error-3"] },
                { "property-2", ["error-1", "error-2"] },
                { "property-3", ["error-1"] }
            };

            var actual = ValidationExceptionHandler.GetProblemDetails(input);
            actual.Extensions["errors"].Must().BeEquivalentTo(expectedDictionary);
        }
    }

    private static ValidationExceptionHandler CreateSut() => new();
}