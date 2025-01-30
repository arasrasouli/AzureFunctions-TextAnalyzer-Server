namespace AzureFunctions_TextAnalyzer.Functions.Dto.Mapper
{
    public interface IDtoMapper<TModel, TDto>
    {
        TModel MapToModel(TDto dto);

        TDto MapFromModel(TModel model);
    }
}
