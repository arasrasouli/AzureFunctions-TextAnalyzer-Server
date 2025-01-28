using AzureFunctions_TextAnalyzer.Functions.Dto;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunctions_TextAnalyzer.Functions
{
    public class TextBlobFunction
    {
        private readonly ILogger<TextBlobFunction> _logger;
        private readonly IConfiguration _config;

        public TextBlobFunction(ILogger<TextBlobFunction> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
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
                int chunkCount = (int)Math.Ceiling((double)blobLength / chunkSize);

                QueueMessageDto[] messageQueue = new QueueMessageDto[chunkCount];
                for (int i = 0; i < chunkCount; i++)
                {
                    long start = i * chunkSize - overlapSize > 0 ? i * chunkSize - overlapSize : 0;
                    long end = Math.Min(start + chunkSize - 1, blobLength - 1);

                    QueueMessageDto message = new QueueMessageDto()
                    {
                        FileName = name,
                        ChunkOrder = i + 1,
                        StartPoint = start,
                        EndPoint = end,
                        LastChunkOrder = chunkCount
                    };

                    messageQueue[i] = message;
                }

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
