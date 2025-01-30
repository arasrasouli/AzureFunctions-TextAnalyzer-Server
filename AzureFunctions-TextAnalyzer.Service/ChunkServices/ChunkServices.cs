//using AzureFunctions_TextAnalyzer.Infrastructure.Helper;
using AzureFunctions_TextAnalyzer.Service.Model;

namespace AzureFunctions_TextAnalyzer.Service
{
    public class ChunkServices : IChunkServices
    {
        private readonly long _chunkSize;
        private readonly int _overlapSize;

        public ChunkServices(long chunkSize, int overlapSize)
        {
            _chunkSize = chunkSize;
            _overlapSize = overlapSize;
        }

        public ChunkDataModel[] GenerateChunkMessages(long blobLength)
        {
            int chunkCount = (int)Math.Ceiling((double)blobLength / this._chunkSize);
            var chunkMessages = new ChunkDataModel[chunkCount];

            for (int i = 0; i < chunkCount; i++)
            {
                long start = Math.Max(i * this._chunkSize - this._overlapSize, 0);
                long end = Math.Min(start + this._chunkSize - 1, blobLength - 1);

                chunkMessages[i] = new ChunkDataModel
                {
                    ChunkIndex = i + 1,
                    StartPoint = start,
                    EndPoint = end,
                    ChunksCount = chunkCount,
                };
            }

            return chunkMessages;
        }
    }
}
