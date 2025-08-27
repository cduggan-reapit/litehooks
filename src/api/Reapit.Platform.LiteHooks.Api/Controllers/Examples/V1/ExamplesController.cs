using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reapit.Platform.ApiVersioning.Attributes;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels.Examples;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels.Examples;
using Reapit.Platform.LiteHooks.Api.Controllers.Shared;
using Reapit.Platform.LiteHooks.Api.Controllers.Shared.Examples;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.DeleteExample;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExampleById;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.PatchExample;
using Swashbuckle.AspNetCore.Filters;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1;

/// <summary>Endpoints used to interact with ExampleEntity objects. </summary>
/// <param name="mediator">The Mediatr sender.</param>
/// <param name="mapper">The AutoMapper service.</param>
[IntroducedInVersion(1, 0)]
[ProducesResponseType<ProblemDetails>(400)]
[SwaggerResponseExample(400, typeof(ApiVersionProblemDetailsExample))]
public class ExamplesController(ISender mediator, IMapper mapper) : ApiControllerBase
{
    /// <summary>Get a page of Examples matching optional filter criteria.</summary>
    /// <param name="model">The filter model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet]
    [ProducesResponseType<ResultPage<ExampleResponseModel>>(200)]
    [ProducesResponseType<ProblemDetails>(400)]
    [SwaggerResponseExample(200, typeof(ExampleResponseModelResultPageExamplesProvider))]
    [SwaggerResponseExample(400, typeof(QueryValidationProblemDetailsExample))]
    public async Task<IActionResult> GetExamples(
        [FromQuery] GetExamplesRequestModel model,
        CancellationToken cancellationToken)
    {
        var query = mapper.Map<GetExamplesQuery>(model);
        var entities = await mediator.Send(query, cancellationToken);
        return Ok(mapper.Map<ResultPage<ExampleResponseModel>>(entities));
    }

    /// <summary>Create a new Example.</summary>
    /// <param name="model">Model describing the object to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPost]
    [ProducesResponseType<ExampleResponseModel>(201)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(CreateExampleRequestModel), typeof(CreateExampleRequestModelExamplesProvider))]
    [SwaggerResponseExample(201, typeof(ExampleResponseModelExamplesProvider))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> CreateExample(
        [FromBody] CreateExampleRequestModel model,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateExampleCommand>(model);
        var entity = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(
            actionName: nameof(GetExampleById),
            routeValues: new { id = entity.Id },
            value: mapper.Map<ExampleResponseModel>(entity));
    }

    /// <summary>Get an Example by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet("{id}")]
    [ProducesResponseType<ExampleResponseModel>(200)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(200, typeof(ExampleDetailResponseModelExamplesProvider))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> GetExampleById(string id, CancellationToken cancellationToken)
    {
        var query = new GetExampleByIdQuery(id);
        var entity = await mediator.Send(query, cancellationToken);
        return Ok(mapper.Map<ExampleDetailResponseModel>(entity));
    }

    /// <summary>Update an Example by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the object.</param>
    /// <param name="model">Model describing the properties to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpPatch("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [ProducesResponseType<ProblemDetails>(422)]
    [SwaggerRequestExample(typeof(PatchExampleRequestModel), typeof(PatchExampleRequestModelExamplesProvider))]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    [SwaggerResponseExample(422, typeof(ValidationProblemDetailsExample))]
    public async Task<IActionResult> PatchExample(
        [FromRoute] string id,
        [FromBody] PatchExampleRequestModel model,
        CancellationToken cancellationToken)
    {
        var query = new PatchExampleCommand(id, model.Name, model.Description);
        _ = await mediator.Send(query, cancellationToken);
        return NoContent();
    }

    /// <summary>Delete an Example by its unique identifier.</summary>
    /// <param name="id">The unique identifier of the object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType<ProblemDetails>(404)]
    [SwaggerResponseExample(404, typeof(NotFoundProblemDetailsExample))]
    public async Task<IActionResult> DeleteExample(string id, CancellationToken cancellationToken)
    {
        var query = new DeleteExampleCommand(id);
        _ = await mediator.Send(query, cancellationToken);
        return NoContent();
    }

    [HttpGet("fail")]
    public Task<IActionResult> Fail()
        => throw new NotImplementedException();
}