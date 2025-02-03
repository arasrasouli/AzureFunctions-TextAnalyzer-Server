using AzureFunctions_TextAnalyzer.Functions.Dto.Mapper;
using Microsoft.Extensions.DependencyInjection;

namespace AzureFunctions_TextAnalyzer.Dto.Mapper
{
    public class MapperFactory : IMapperFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MapperFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDtoMapper<TModel, TDto> GetMapper<TModel, TDto>()
        {
            return _serviceProvider.GetRequiredService<IDtoMapper<TModel, TDto>>();
        }
    }
}
