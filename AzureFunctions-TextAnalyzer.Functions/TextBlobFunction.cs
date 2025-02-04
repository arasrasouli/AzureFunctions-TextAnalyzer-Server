using AzureFunctions_TextAnalyzer.Dto.Mapper;
using AzureFunctions_TextAnalyzer.Functions.Dto;
using AzureFunctions_TextAnalyzer.Functions.Dto.Mapper;
using AzureFunctions_TextAnalyzer.Common.Enums;
using AzureFunctions_TextAnalyzer.Service;
using AzureFunctions_TextAnalyzer.Service.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AzureFunctions_TextAnalyzer.Functions
{
    public class TextBlobFunction
    {
        private readonly ILogger<TextBlobFunction> _logger;
        private readonly IConfiguration _config;
        private readonly IChunkServices _chunkService;
        private readonly IMapperFactory _mapperFactory;

        public TextBlobFunction(
            ILogger<TextBlobFunction> logger,
            IConfiguration config,
            IChunkServices chunkService,
            IMapperFactory mapperFactory
        )
        {
            _logger = logger;
            _config = config;
            _chunkService = chunkService;
            _mapperFactory = mapperFactory;
        }

        [Function("OnBlobUploadGenerateChunks")]
        [QueueOutput("blob-chunk-queue")]
        public async Task<ChunkQueueMessageDto[]> OnBlobUploadGenerateChunks(
            [BlobTrigger("txt-container/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name)
        {
            try
            {
                _logger.LogInformation($"Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

                long blobLength = myBlob.Length;
                IDtoMapper<ChunkDataModel, ChunkQueueMessageDto> chunkMapper = _mapperFactory.GetMapper<ChunkDataModel, ChunkQueueMessageDto>();

                ChunkQueueMessageDto[] messageQueue = (await _chunkService
                    .GenerateChunkMessagesAsync(name, blobLength))
                    .Select(item => chunkMapper.MapFromModel(item)).ToArray();

                return messageQueue;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message.ToString());
                throw;
            }
        }

        [Function("ProcessChunkQueueMessage")]
        [QueueOutput("chunk-word-count-queue")]
        public async Task<FileChunkDto?> ProcessChunkQueueMessage(
            [QueueTrigger("blob-chunk-queue", Connection = "")] string message)
        {
            try
            {
                if (message == null)
                {
                    _logger.LogInformation($"null message");
                    return null;
                }

                ChunkQueueMessageDto chunkMessageDto = JsonSerializer.Deserialize<ChunkQueueMessageDto>(message);

                IDtoMapper<ChunkDataModel, ChunkQueueMessageDto> chunkMapper = _mapperFactory.GetMapper<ChunkDataModel, ChunkQueueMessageDto>();
                FileChunkModel fileProcessStatus = await _chunkService.ProcessChunkAsync(chunkMapper.MapToModel(chunkMessageDto));
                _logger.LogInformation($"Processed chunk {chunkMessageDto.ChunkIndex} for file {chunkMessageDto.FileName}");

                IDtoMapper<FileChunkModel, FileChunkDto> fileChunkMapper = _mapperFactory.GetMapper<FileChunkModel, FileChunkDto>();
                return fileProcessStatus.Status == FileProcessingStatus.Completed.ToString() ? fileChunkMapper.MapFromModel(fileProcessStatus) : null;                   
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing queue message: {ex.Message}");
                throw;
            }
        }
    }
}