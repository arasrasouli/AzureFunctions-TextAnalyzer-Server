﻿using AzureFunctions_TextAnalyzer.DAL.Entities;

namespace AzureFunctions_TextAnalyzer.DAL.Repositories
{
    public interface IChunkWordRepository : ITableStorageRepository<ChunkWordEntity>
    {
        Task<List<string>> GetChunksAsync(string partitionKey);
    }
}