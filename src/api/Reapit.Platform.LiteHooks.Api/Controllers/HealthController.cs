using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.LiteHooks.Api.Controllers.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace Reapit.Platform.LiteHooks.Api.Controllers;

/// <summary>Endpoints for checking the status of the service</summary>
public class HealthController : ApiControllerBase
{
    /// <summary>Endpoint used to confirm service is live.</summary>
    [HttpGet]
    [SwaggerIgnore]
    [ApiVersionNeutral]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ProducesResponseType(204)]
    public IActionResult HealthCheck()
        => NoContent();
}