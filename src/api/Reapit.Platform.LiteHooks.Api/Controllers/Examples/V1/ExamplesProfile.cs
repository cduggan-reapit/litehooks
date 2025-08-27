using AutoMapper;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.RequestModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1.ResponseModels;
using Reapit.Platform.LiteHooks.Api.Controllers.Shared;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.CreateExample;
using Reapit.Platform.LiteHooks.Core.UseCases.Examples.GetExamples;
using Reapit.Platform.LiteHooks.Domain.Entities;

namespace Reapit.Platform.LiteHooks.Api.Controllers.Examples.V1;

/// <summary>AutoMapper configuration profile for Example endpoints.</summary>
public class ExamplesProfile : Profile
{
    /// <summary>Initializes a new instance of the <see cref="ExamplesProfile"/> class.</summary>
    public ExamplesProfile()
    {
        // ExampleEntity => ExampleResponseModel
        CreateMap<ExampleEntity, ExampleResponseModel>()
            .ForCtorParam(nameof(ExampleResponseModel.Id), ops => ops.MapFrom(source => source.Id))
            .ForCtorParam(nameof(ExampleResponseModel.Name), ops => ops.MapFrom(source => source.Name))
            .ForCtorParam(nameof(ExampleResponseModel.Description), ops => ops.MapFrom(source => source.Description));

        // ExampleEntity => ExampleDetailResponseModel
        CreateMap<ExampleEntity, ExampleDetailResponseModel>()
            .ForCtorParam(nameof(ExampleDetailResponseModel.Id), ops => ops.MapFrom(source => source.Id))
            .ForCtorParam(nameof(ExampleDetailResponseModel.Name), ops => ops.MapFrom(source => source.Name))
            .ForCtorParam(nameof(ExampleDetailResponseModel.Description), ops => ops.MapFrom(source => source.Description))
            .ForCtorParam(nameof(ExampleDetailResponseModel.DateCreated), ops => ops.MapFrom(source => source.DateCreated))
            .ForCtorParam(nameof(ExampleDetailResponseModel.DateModified), ops => ops.MapFrom(source => source.DateModified));

        // IEnumerable<ExampleEntity> => ResultPage<ExampleResponseModel>
        CreateMap<IEnumerable<ExampleEntity>, ResultPage<ExampleResponseModel>>()
            .ConvertUsing<ResultPageConverter<ExampleEntity, ExampleResponseModel>>();

        // GetExamplesRequestModel => GetExamplesQuery
        CreateMap<GetExamplesRequestModel, GetExamplesQuery>()
            .ForCtorParam(nameof(GetExamplesQuery.Cursor), ops => ops.MapFrom(source => source.Cursor))
            .ForCtorParam(nameof(GetExamplesQuery.PageSize), ops => ops.MapFrom(source => source.PageSize ?? 25))
            .ForCtorParam(nameof(GetExamplesQuery.Name), ops => ops.MapFrom(source => source.Name))
            .ForCtorParam(nameof(GetExamplesQuery.Description), ops => ops.MapFrom(source => source.Description))
            .ForCtorParam(nameof(GetExamplesQuery.CreatedFrom), ops => ops.MapFrom(source => source.CreatedFrom))
            .ForCtorParam(nameof(GetExamplesQuery.CreatedTo), ops => ops.MapFrom(source => source.CreatedTo))
            .ForCtorParam(nameof(GetExamplesQuery.ModifiedFrom), ops => ops.MapFrom(source => source.ModifiedFrom))
            .ForCtorParam(nameof(GetExamplesQuery.ModifiedTo), ops => ops.MapFrom(source => source.ModifiedTo));

        // CreateExampleRequestModel => CreateExampleCommand
        CreateMap<CreateExampleRequestModel, CreateExampleCommand>()
            .ForCtorParam(nameof(CreateExampleCommand.Name), ops => ops.MapFrom(source => source.Name))
            .ForCtorParam(nameof(CreateExampleCommand.Description), ops => ops.MapFrom(source => source.Description));
    }
}