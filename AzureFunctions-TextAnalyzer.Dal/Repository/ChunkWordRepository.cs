using Azure.Data.Tables;
using AzureFunctions_TextAnalyzer.DAL.Entities;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories
{
    public class ChunkWordRepository : TableStorageRepository<ChunkWordEntity>, IChunkWordRepository
    {
        public ChunkWordRepository(TableClient tableClient)
            : base(tableClient)
        {
        }
    }
}
