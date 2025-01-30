using AzureFunctions_TextAnalyzer.Service.Model;

namespace AzureFunctions_TextAnalyzer.Service
{
    public interface IChunkServices
    {
        ChunkDataModel[] GenerateChunkMessages(long blobLength);
    }
}
