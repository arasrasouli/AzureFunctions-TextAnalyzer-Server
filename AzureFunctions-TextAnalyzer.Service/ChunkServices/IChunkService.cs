using AzureFunctions_TextAnalyzer.Service.Model;

namespace AzureFunctions_TextAnalyzer.Service
{
    public interface IChunkServices
    {
        Task<ChunkDataModel[]> GenerateChunkMessagesAsync(string fileName, long fileLength);
    }
}
