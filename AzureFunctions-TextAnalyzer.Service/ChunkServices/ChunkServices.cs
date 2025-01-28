using AzureFunctions_TextAnalyzer.Service.Model;

namespace AzureFunctions_TextAnalyzer.Service
{
    public class ChunkServices : IChunkServices
    {
        public ChunkDataModel[] GenerateChunkMessages(long blobLength, int chunkSize, int overlapSize)
        {
            int chunkCount = (int)Math.Ceiling((double)blobLength / chunkSize);
            var chunkMessages = new ChunkDataModel[chunkCount];

            for (int i = 0; i < chunkCount; i++)
            {
                long start = Math.Max(i * chunkSize - overlapSize, 0);
                long end = Math.Min(start + chunkSize - 1, blobLength - 1);

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
