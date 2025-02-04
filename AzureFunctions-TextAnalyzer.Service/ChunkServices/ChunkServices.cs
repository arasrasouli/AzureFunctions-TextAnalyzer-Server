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

        public async Task<FileChunkModel> ProcessChunkAsync(ChunkDataModel chunk)
        {
            try
            {
                string originalChunk = await _fileServices.ReadBlobChunkAsync(chunk.Name, chunk.StartPoint, chunk.EndPoint);

                string NonWordWithSpaceChunk = StringHelper.ReplaceNonWordWithSpace(originalChunk);

                string validChunk = GetValidChunk(NonWordWithSpaceChunk, chunk.ChunkIndex == 1, chunk.ChunkIndex == chunk.ChunksCount);

                Dictionary<string, int> wordsCount = CountWords(validChunk);

                string partitionKey = StringHelper.GeneratePartitionKey(chunk.Name, chunk.ChunksCount);

                await _chunkWordRepository.AddEntityAsync(new ChunkWordEntity()
                {
                    PartitionKey = partitionKey,
                    RowKey = $"index_{chunk.ChunkIndex}",
                    WordsCount = JsonSerializer.Serialize(wordsCount)
                });

                FileChunkEntity processedFileChunk = await _fileChunkRepository.UpdateRemainingChunksAsync(partitionKey, chunk.Name, chunk.ChunksCount) ?? new FileChunkEntity();

                return new FileChunkModel()
                {
                    PartitionKey = partitionKey,
                    Status = processedFileChunk.RemainingChunks == 0 ? FileProcessingStatus.Completed.ToString() : FileProcessingStatus.InProgress.ToString(),
                    RowKey = chunk.Name,
                };
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        protected string GetValidChunk(string chunk, bool isFirst, bool isLast)
        {
            if (!isFirst && isLast && chunk.Length <= this._overlapSize)
            {
                return string.Empty;
            }

            int startPoint = isFirst ? 0 : StringHelper.FindIndexOfLastSpace(chunk.Substring(0, this._overlapSize));

            int distanceFromEndPoint = isLast ? 0 : this._overlapSize - StringHelper.FindIndexOfLastSpace(chunk.Substring(chunk.Length - this._overlapSize, this._overlapSize));

            return chunk.Substring(startPoint, chunk.Length - distanceFromEndPoint - startPoint);
        }

        protected Dictionary<string, int> CountWords(string chunk)
        {
            var wordCounts = new Dictionary<string, int>();
            var words = chunk.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (wordCounts.ContainsKey(word))
                    wordCounts[word]++;
                else
                    wordCounts[word] = 1;
            }

            return wordCounts;
        }
    }
}
