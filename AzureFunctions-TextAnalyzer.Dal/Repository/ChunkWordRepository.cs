using AzureFunctions_TextAnalyzer.DAL.Entities;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories
{
    public class ChunkWordRepository : TableStorageRepository<ChunkWordEntity>, IChunkWordRepository
    {
        public ChunkWordRepository(string connectionString, string tableName)
            : base(connectionString, tableName)
        {
        }
    }
}
