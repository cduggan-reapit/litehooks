using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Reapit.Platform.LiteHooks.Api.ExceptionHandlers;

/// <summary>Handler for <see cref="ValidationException"/> and its derived types.</summary>
public class ValidationExceptionHandler : IExceptionHandler
{
    internal const string Title = "Validation Failed";
    internal const string Type = "https://www.reapit.com/errors/validation";
    internal const string Detail = "One or more validation errors occurred.";
    internal const int StatusCode = 422;

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException resolvedException)
            return false;

        httpContext.Response.StatusCode = StatusCode;
        await httpContext.Response.WriteAsJsonAsync(GetProblemDetails(resolvedException), cancellationToken);
        return true; ;
    }

    /// <summary>Get the <see cref="ProblemDetails"/> object representing a given <see cref="ValidationException"/>.</summary>
    /// <param name="exception">The exception to represent.</param>
    internal static ProblemDetails GetProblemDetails(ValidationException exception)
    {
        var problemDetails = new ProblemDetails
        {
            Title = Title,
            Type = Type,
            Detail = Detail,
            Status = StatusCode,
        };

        if (!exception.Errors.Any())
            return problemDetails;

        problemDetails.Extensions.Add("errors", exception.Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(
                keySelector: group => JsonNamingPolicy.CamelCase.ConvertName(group.Key),
                elementSelector: group => group.Select(item => item.ErrorMessage)));

        return problemDetails;
    }
}