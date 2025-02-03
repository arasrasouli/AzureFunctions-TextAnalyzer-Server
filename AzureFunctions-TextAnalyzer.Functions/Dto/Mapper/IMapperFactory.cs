using AzureFunctions_TextAnalyzer.Functions.Dto.Mapper;

namespace AzureFunctions_TextAnalyzer.Dto.Mapper
{
    public interface IMapperFactory
    {
        IDtoMapper<TModel, TDto> GetMapper<TModel, TDto>();
    }
}