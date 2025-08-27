using Reapit.Platform.Common.Providers.Identifiers;
using Reapit.Platform.Common.Providers.Temporal;
using Reapit.Platform.Internal.Common.Pagination;
using Reapit.Platform.Testing.Fluent;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels;
using Reapit.Platform.LiteHooks.Data.Context;
using Reapit.Platform.LiteHooks.Domain.Entities;
using System.Net;

namespace Reapit.Platform.LiteHooks.Api.IntegrationTests.Controllers.Examples.V1;

public static class ExamplesControllerTests
{
    public class GetExamples(TestApiFactory apiFactory) : ExamplesControllerTestsBase(apiFactory)
    {
        [Theory]
        [InlineData(null, ProblemDetailsTypes.UnspecifiedApiVersion)]
        [InlineData("0.1", ProblemDetailsTypes.UnsupportedApiVersion)]
        [InlineData("2.0", ProblemDetailsTypes.UnsupportedApiVersion)]
        public async Task Should_ReturnBadRequest_WhenVersionInvalid(string? version, string expectedError)
        {
            var response = await CreateRequest(HttpMethod.Get, BasePath)
                .SetHeader(ApiVersionHeader, version)
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            // Keep an eye out, we're planning on implementing a new method in the testing package that would allow this
            // to be defined as `response.Must().HaveStatusCode(...).And.BeProblemDetails().WhichMust.Title(...);`
            response.Must().HaveStatusCode(HttpStatusCode.BadRequest).And.BeProblemDetails(out var problemDetails);
            problemDetails.Must().HaveTitle(expectedError);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenQueryStringInvalid()
        {
            var response = await CreateRequest(HttpMethod.Get, BasePath + "?pageSize=999")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.BadRequest)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(ProblemDetailsTypes.BadRequest);
        }

        [Fact]
        public async Task Should_ReturnOk_WithPagedResults()
        {
            await InitializeDatabaseAsync();

            var firstPageEntities = SeedData.OrderBy(o => o.Cursor).Take(10).ToList();
            var secondPageEntities = SeedData.OrderBy(o => o.Cursor).Skip(10).Take(10);
            var secondPageOffset = firstPageEntities.MaxBy(d => d.Cursor)!.Cursor;

            var expectedFirstPage = firstPageEntities.ToResultPage(ExampleResponseModel.FromEntity);
            var expectedSecondPage = secondPageEntities.ToResultPage(ExampleResponseModel.FromEntity);

            var firstResponse = await CreateRequest(HttpMethod.Get, BasePath + "?pageSize=10")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            firstResponse.Must().HaveStatusCode(HttpStatusCode.OK)
                .And.HaveJsonContent(expectedFirstPage);

            var secondResponse = await CreateRequest(HttpMethod.Get, $"{BasePath}?pageSize=10&cursor={secondPageOffset}")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            secondResponse.Must().HaveStatusCode(HttpStatusCode.OK)
                .And.HaveJsonContent(expectedSecondPage);
        }
    }

    public class CreateExample(TestApiFactory apiFactory) : ExamplesControllerTestsBase(apiFactory)
    {
        [Theory]
        [InlineData(null, ProblemDetailsTypes.UnspecifiedApiVersion)]
        [InlineData("0.1", ProblemDetailsTypes.UnsupportedApiVersion)]
        [InlineData("2.0", ProblemDetailsTypes.UnsupportedApiVersion)]
        public async Task Should_ReturnBadRequest_WhenVersionInvalid(string? version, string expectedError)
        {
            var response = await CreateRequest(HttpMethod.Post, BasePath)
                .SetHeader(ApiVersionHeader, version)
                .SetStringContent(new CreateExampleRequestModel("name", "description"))
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.BadRequest)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(expectedError);
        }

        [Fact]
        public async Task Should_ReturnUnprocessable_WhenValidationFailed()
        {
            var response = await CreateRequest(HttpMethod.Post, BasePath)
                .SetHeader(ApiVersionHeader, "1.0")
                .SetStringContent(new CreateExampleRequestModel(new string('a', 101), null))
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.UnprocessableContent)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(ProblemDetailsTypes.ValidationFailed);
        }
        
        [Fact]
        public async Task Should_ReturnCreated_WithEntityModel()
        {
            await InitializeDatabaseAsync();
            
            const string name = "new entity", description = "new entity description";
            var response = await CreateRequest(HttpMethod.Post, BasePath)
                .SetHeader(ApiVersionHeader, "1.0")
                .SetStringContent(new CreateExampleRequestModel(name, description))
                .SendAsync<TestApiFactory, Program>(ApiFactory);
            
            response.Must().HaveStatusCode(HttpStatusCode.Created);
            var responseContent = await response.Content.ReadFromJsonAsync<ExampleResponseModel>();
            responseContent.Must().NotBeNull()
                .And.Match<ExampleResponseModel>(model => model.Name == name && model.Description == description);
            
            // Check the return url is right (it'll have the host and whatnot, so we only care about the ending)...
            var checkUrl = $"{BasePath}/{responseContent!.Id}";
            var location = response.Headers.GetValues("Location").First();
            location.Must().EndWith(checkUrl, StringComparison.OrdinalIgnoreCase);
            
            // ... and check that the created object is accessible
            var checkResponse = await CreateRequest(HttpMethod.Get, $"{BasePath}/{responseContent.Id}")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);
            checkResponse.Must().HaveStatusCode(HttpStatusCode.OK);
        }
    }

    public class GetExampleById(TestApiFactory apiFactory) : ExamplesControllerTestsBase(apiFactory)
    {
        [Theory]
        [InlineData(null, ProblemDetailsTypes.UnspecifiedApiVersion)]
        [InlineData("0.1", ProblemDetailsTypes.UnsupportedApiVersion)]
        [InlineData("2.0", ProblemDetailsTypes.UnsupportedApiVersion)]
        public async Task Should_ReturnBadRequest_WhenVersionInvalid(string? version, string expectedError)
        {
            var response = await CreateRequest(HttpMethod.Get, $"{BasePath}/001")
                .SetHeader(ApiVersionHeader, version)
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.BadRequest)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(expectedError);
        }
        
        [Fact]
        public async Task Should_ReturnNotFound_WhenResourceDoesNotExist()
        {
            await InitializeDatabaseAsync();
            
            var response = await CreateRequest(HttpMethod.Get, $"{BasePath}/999")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.NotFound)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(ProblemDetailsTypes.ResourceNotFound);
        }
        
        [Fact]
        public async Task Should_ReturnDetailedObject_WhenResourceFound()
        {
            const string id = "053";
            
            await InitializeDatabaseAsync();

            var entity = SeedData.Single(i => i.Id == id);
            var expected = ExampleDetailResponseModel.FromEntity(entity);
            
            var response = await CreateRequest(HttpMethod.Get, $"{BasePath}/{id}")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);
            
            response.Must().HaveStatusCode(HttpStatusCode.OK)
                .And.HaveJsonContent(expected);
        }
    }

    public class PatchExample(TestApiFactory apiFactory) : ExamplesControllerTestsBase(apiFactory)
    {
        [Theory]
        [InlineData(null, ProblemDetailsTypes.UnspecifiedApiVersion)]
        [InlineData("0.1", ProblemDetailsTypes.UnsupportedApiVersion)]
        [InlineData("2.0", ProblemDetailsTypes.UnsupportedApiVersion)]
        public async Task Should_ReturnBadRequest_WhenVersionInvalid(string? version, string expectedError)
        {
            var response = await CreateRequest(HttpMethod.Patch, $"{BasePath}/047")
                .SetHeader(ApiVersionHeader, version)
                .SetStringContent(new PatchExampleRequestModel("new name"))
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.BadRequest)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(expectedError);
        }
        
        [Fact]
        public async Task Should_ReturnNotFound_WhenResourceDoesNotExist()
        {
            await InitializeDatabaseAsync();
            
            var response = await CreateRequest(HttpMethod.Patch, $"{BasePath}/999")
                .SetHeader(ApiVersionHeader, "1.0")
                .SetStringContent(new PatchExampleRequestModel())
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.NotFound)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(ProblemDetailsTypes.ResourceNotFound);
        }
        
        [Fact]
        public async Task Should_ReturnUnprocessable_WhenValidationFailed()
        {
            await InitializeDatabaseAsync();
            
            var response = await CreateRequest(HttpMethod.Patch, $"{BasePath}/042")
                .SetHeader(ApiVersionHeader, "1.0")
                .SetStringContent(new PatchExampleRequestModel(new string('a', 101)))
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.UnprocessableContent)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(ProblemDetailsTypes.ValidationFailed);
        }
        
        [Fact]
        public async Task Should_ReturnNoContent()
        {
            const string id = "042", name = "new name 042";
            await InitializeDatabaseAsync();
            
            var response = await CreateRequest(HttpMethod.Patch, $"{BasePath}/{id}")
                .SetHeader(ApiVersionHeader, "1.0")
                .SetStringContent(new PatchExampleRequestModel(name))
                .SendAsync<TestApiFactory, Program>(ApiFactory);
            
            response.Must().HaveStatusCode(HttpStatusCode.NoContent);
            
            var check = await CreateRequest(HttpMethod.Get, $"{BasePath}/{id}")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);
            var checkContent = await check.Content.ReadFromJsonAsync<ExampleDetailResponseModel>();
            checkContent.Must().NotBeNull()
                .And.Match<ExampleDetailResponseModel>(content => content.Name == name);
        }
    }

    public class DeleteExample(TestApiFactory apiFactory) : ExamplesControllerTestsBase(apiFactory)
    {
        [Theory]
        [InlineData(null, ProblemDetailsTypes.UnspecifiedApiVersion)]
        [InlineData("0.1", ProblemDetailsTypes.UnsupportedApiVersion)]
        [InlineData("2.0", ProblemDetailsTypes.UnsupportedApiVersion)]
        public async Task Should_ReturnBadRequest_WhenVersionInvalid(string? version, string expectedError)
        {
            var response = await CreateRequest(HttpMethod.Delete, $"{BasePath}/047")
                .SetHeader(ApiVersionHeader, version)
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.BadRequest)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(expectedError);
        }
        
        [Fact]
        public async Task Should_ReturnNotFound_WhenResourceDoesNotExist()
        {
            await InitializeDatabaseAsync();
            
            var response = await CreateRequest(HttpMethod.Delete, $"{BasePath}/999")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);

            response.Must().HaveStatusCode(HttpStatusCode.NotFound)
                .And.BeProblemDetails(out var problemDetails);

            problemDetails.Must().HaveTitle(ProblemDetailsTypes.ResourceNotFound);
        }
        
        [Fact]
        public async Task Should_ReturnNoContent()
        {
            const string id = "056";
            await InitializeDatabaseAsync();
            
            var response = await CreateRequest(HttpMethod.Delete, $"{BasePath}/{id}")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);
            response.Must().HaveStatusCode(HttpStatusCode.NoContent);

            var check = await CreateRequest(HttpMethod.Get, $"{BasePath}/{id}")
                .SetHeader(ApiVersionHeader, "1.0")
                .SendAsync<TestApiFactory, Program>(ApiFactory);
            check.Must().HaveStatusCode(HttpStatusCode.NotFound);
        }
    }
    
    public abstract class ExamplesControllerTestsBase(TestApiFactory apiFactory)
        : ApiIntegrationTestBase<TestApiFactory, Program>(apiFactory)
    {
        protected const string ApiVersionHeader = "x-api-version";

        protected const string BasePath = "api/examples";
        
        protected static readonly IEnumerable<ExampleEntity> SeedData = GetSeedData();

        private static readonly DateTimeOffset BaseDate = new(2020, 1, 1, 12, 0, 0, TimeSpan.Zero);
        
        protected async Task InitializeDatabaseAsync()
        {
            await using var scope = ApiFactory.Services.CreateAsyncScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
            _ = await context.Database.EnsureDeletedAsync();
            _ = await context.Database.EnsureCreatedAsync();

            await context.Examples.AddRangeAsync(SeedData);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        private static IEnumerable<ExampleEntity> GetSeedData()
            => Enumerable
                .Range(1, 100)
                .Select(seed =>
                {
                    using var idFix = new IdentifierProviderContext($"{seed:D3}");
                    using var dtFix = new DateTimeOffsetProviderContext(BaseDate.AddDays(seed - 1));
                    return new ExampleEntity($"name-{seed:D3}", $"description-{seed:D3}");
                });
    }
}