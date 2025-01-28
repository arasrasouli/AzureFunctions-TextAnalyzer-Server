using AzureFunctions_TextAnalyzer.Functions.Dto;
using AzureFunctions_TextAnalyzer.Service;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunctions_TextAnalyzer.Functions
{
    public class TextBlobFunction
    {
        private readonly ILogger<TextBlobFunction> _logger;
        private readonly IConfiguration _config;
        private readonly IChunkServices _chunkService;

        public TextBlobFunction(ILogger<TextBlobFunction> logger, IConfiguration config, IChunkServices chunkService)
        {
            _logger = logger;
            _config = config;
            _chunkService = chunkService;
        }

        [Function("OnBlobUploadGenerateChunks")]
        [QueueOutput("blob-chunk-queue")]
        public QueueMessageDto[] OnBlobUploadGenerateChunks(
            [BlobTrigger("txt-container/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name)
        {
            try
            {
                _logger.LogInformation($"Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

                int chunkSize = _config.GetValue<int>(Literals.ChunkSize);
                int overlapSize = _config.GetValue<int>(Literals.OverlapSize);
                long blobLength = myBlob.Length;

                QueueMessageDto[] messageQueue = _chunkService.GenerateChunkMessages(blobLength, chunkSize, overlapSize)
                    .Select(x=> new QueueMessageDto()
                    {
                        FileName = name,
                        ChunkIndex = x.ChunkIndex,
                        StartPoint = x.StartPoint,
                        EndPoint = x.EndPoint,
                        ChunksCount = x.ChunksCount

                    }).ToArray();

                return messageQueue;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                throw;
            }
        }

    }
}
