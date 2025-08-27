using Microsoft.AspNetCore.Mvc;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Shared;

/// <summary>Base controller class for API controllers.</summary>
[ApiController]
[Route("/api/[controller]")]
public abstract class ApiControllerBase : ControllerBase;