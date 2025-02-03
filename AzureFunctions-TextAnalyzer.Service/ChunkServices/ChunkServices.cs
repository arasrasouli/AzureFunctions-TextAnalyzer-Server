using AzureFunctions_TextAnalyzer.DAL.Entities;
using AzureFunctions_TextAnalyzer.DAL.Repositories;
using AzureFunctions_TextAnalyzer.Common;
using AzureFunctions_TextAnalyzer.Common.Enums;
using AzureFunctions_TextAnalyzer.Common.Helper;
using AzureFunctions_TextAnalyzer.Service.Model;
using System.Text.Json;

namespace AzureFunctions_TextAnalyzer.Service
{
    public class ChunkServices : IChunkServices
    {
        private readonly IChunkWordRepository _chunkWordRepository;
        private readonly IFileChunkRepository _fileChunkRepository;
        private readonly IFileServices _fileServices;
        private readonly long _chunkSize;
        private readonly int _overlapSize;

        public ChunkServices(IChunkWordRepository chunkWordRepository, IFileChunkRepository fileChunkRepository, IFileServices fileServices, long chunkSize, int overlapSize)
        {
            _chunkWordRepository = chunkWordRepository;
            _fileChunkRepository = fileChunkRepository;
            _fileServices = fileServices;
            _chunkSize = chunkSize;
            _overlapSize = overlapSize;
        }

        public async Task<ChunkDataModel[]> GenerateChunkMessagesAsync(string fileName, long fileLength)
        {
            int chunksCount = (int)Math.Ceiling((double)fileLength / this._chunkSize);
            var chunkMessages = new ChunkDataModel[chunksCount];

            await _fileChunkRepository.AddEntityAsync(new FileChunkEntity()
            {
                PartitionKey = StringHelper.GeneratePartitionKey(fileName, chunksCount),
                RowKey = fileName,
                RemainingChunks = chunksCount,
            });

            for (int i = 0; i < chunksCount; i++)
            {
                long start = Math.Max(i * this._chunkSize - this._overlapSize, 0);
                long end = Math.Min(start + this._chunkSize - 1, fileLength - 1);

                chunkMessages[i] = new ChunkDataModel
                {
                    Name = fileName,
                    ChunkIndex = i + 1,
                    StartPoint = start,
                    EndPoint = end,
                    ChunksCount = chunksCount,
                };
            }

            return chunkMessages;
        }
    }
}
