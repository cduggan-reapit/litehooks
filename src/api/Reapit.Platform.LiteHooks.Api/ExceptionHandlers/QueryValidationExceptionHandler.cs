using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.LiteHooks.Core.Exceptions;
using System.Text.Json;

namespace Reapit.Platform.LiteHooks.Api.ExceptionHandlers;

/// <summary>Handler for <see cref="QueryValidationException"/> and its derived types.</summary>
public class QueryValidationExceptionHandler : IExceptionHandler
{
    internal const string Title = "Bad Request";
    internal const string Type = "https://www.reapit.com/errors/bad-request";
    internal const int StatusCode = 400;

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not QueryValidationException resolvedException)
            return false;

        httpContext.Response.StatusCode = StatusCode;
        await httpContext.Response.WriteAsJsonAsync(GetProblemDetails(resolvedException), cancellationToken);
        return true; ;
    }

    /// <summary>Get the <see cref="ProblemDetails"/> object representing a given <see cref="QueryValidationException"/>.</summary>
    /// <param name="exception">The exception to represent.</param>
    internal static ProblemDetails GetProblemDetails(QueryValidationException exception)
    {
        var problemDetails = new ProblemDetails
        {
            Title = Title,
            Type = Type,
            Detail = exception.Message,
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