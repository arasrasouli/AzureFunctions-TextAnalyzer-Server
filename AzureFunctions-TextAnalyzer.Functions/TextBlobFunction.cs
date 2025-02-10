using Azure.Messaging.ServiceBus;
using AzureFunctions_TextAnalyzer.Common.Enums;
using AzureFunctions_TextAnalyzer.Dto.Mapper;
using AzureFunctions_TextAnalyzer.Functions.Dto;
using AzureFunctions_TextAnalyzer.Functions.Dto.Mapper;
using AzureFunctions_TextAnalyzer.Service;
using AzureFunctions_TextAnalyzer.Service.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AzureFunctions_TextAnalyzer.Functions
{
    public class TextBlobFunction
    {
        private readonly ILogger<TextBlobFunction> _logger;
        private readonly IChunkServices _chunkService;
        private readonly IMapperFactory _mapperFactory;

        public TextBlobFunction(
            ILogger<TextBlobFunction> logger,
            IChunkServices chunkService,
            IMapperFactory mapperFactory
        )
        {
            _logger = logger;
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
            [QueueTrigger("blob-chunk-queue", Connection = "AzureWebJobsStorage")] string message)
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

        [Function("SummarizeChunksFunction")]
        public async Task SummarizeChunksFunction(
            [QueueTrigger("chunk-word-count-queue", Connection = "AzureWebJobsStorage")] string message)
        {
            if (string.IsNullOrWhiteSpace(message)) 
            {
                _logger.LogInformation("Received null or empty message.");
                return;
            }

            try
            {
                FileChunkDto fileChunkMessageDto = JsonSerializer.Deserialize<FileChunkDto>(message);

                if (fileChunkMessageDto == null)
                {
                    _logger.LogError("Failed to deserialize message: {Message}", message);
                    return;
                }

                IDtoMapper<FileChunkModel, FileChunkDto> fileChunkMapper = _mapperFactory.GetMapper<FileChunkModel, FileChunkDto>();
                FileChunkModel fileChunkModel = fileChunkMapper.MapToModel(fileChunkMessageDto);

                _logger.LogInformation("Summarizing chunks for PartitionKey: {PartitionKey}, RowKey: {RowKey}", fileChunkModel.PartitionKey, fileChunkModel.RowKey);

                bool summarizationSuccessful = await _chunkService.SummarizeChunks(fileChunkModel);

                if (!summarizationSuccessful)
                {
                    _logger.LogError("Summarization failed for PartitionKey: {PartitionKey}, RowKey: {RowKey}.  Message: {Message}", fileChunkModel.PartitionKey, fileChunkModel.RowKey, message);
                    return;
                }

                _logger.LogInformation("Summarization and message publishing completed successfully for PartitionKey: {PartitionKey}, RowKey: {RowKey}", fileChunkModel.PartitionKey, fileChunkModel.RowKey);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing message: {Message}", message);
            }
            catch (ServiceBusException ex)
            {
                _logger.LogError(ex, "Error processing message (Service Bus related): {Message}", message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing message: {Message}", message);
            }
        }
    }
}