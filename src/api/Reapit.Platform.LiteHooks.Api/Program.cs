using Reapit.Platform.ApiVersioning;
using Reapit.Platform.ApiVersioning.Options;
using Reapit.Platform.Common;
using Reapit.Platform.ErrorHandling;
using Reapit.Platform.Swagger;
using Reapit.Platform.Swagger.Configuration;
using Reapit.Platform.LiteHooks.Api.ExceptionHandlers;
using Reapit.Platform.LiteHooks.Api.Infrastructure.Logging;
using Reapit.Platform.LiteHooks.Core;
using Reapit.Platform.LiteHooks.Data;

const string apiVersionHeader = "x-api-version";

var builder = WebApplication.CreateBuilder(args);
builder.AddLoggingServices();

// Add services from Reapit packages
// Registration for these will switch target IHostApplicationBuilder rather than IServiceCollection, keep an eye out!
builder.Services.AddCommonServices()
    .AddRangedApiVersioning(typeof(Reapit.Platform.LiteHooks.Api.Program).Assembly, new VersionedApiOptions { ApiVersionHeader = apiVersionHeader })
    .AddReapitSwagger(new ReapitSwaggerOptions
    {
        ApiVersionHeader = apiVersionHeader,
        DocumentTitle = "Reapit Demo API",
        UseXmlDocumentation = true
    })
    .AddExceptionHandler<ValidationExceptionHandler>()
    .AddExceptionHandler<QueryValidationExceptionHandler>()
    .RegisterDefaultExceptionHandlers();

// Add services from other projects in this solution
builder.AddCoreServices()
    .AddDataServices();

// Add services for the Api project
builder.Services.AddAutoMapper(typeof(Reapit.Platform.LiteHooks.Api.Program).Assembly);

// Register controllers for routing
builder.Services.AddControllers();

var app = builder.Build();

app.UseReapitSwagger()
    .UseRangedApiVersioning()
    .UseExceptionHandlers()
    .UseRouting();

app.MapControllers();

app.Run();

namespace Reapit.Platform.LiteHooks.Api
{
    /// <summary>Class description allowing test service injection.</summary>
    public partial class Program { }
}