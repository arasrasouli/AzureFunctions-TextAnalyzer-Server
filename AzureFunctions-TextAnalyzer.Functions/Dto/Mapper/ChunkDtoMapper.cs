using AzureFunctions_TextAnalyzer.Service.Model;


namespace AzureFunctions_TextAnalyzer.Functions.Dto.Mapper
{
    public class ChunkDtoMapper : IDtoMapper<ChunkDataModel, ChunkQueueMessageDto>
    {
        public ChunkQueueMessageDto MapFromModel(ChunkDataModel model) => model == null
          ? new ChunkQueueMessageDto()
          : new ChunkQueueMessageDto
          {
              FileName = model.Name,
              ChunkIndex = model.ChunkIndex,
              ChunksCount = model.ChunksCount,
              StartPoint = model.StartPoint,
              EndPoint = model.EndPoint
          };

        public ChunkDataModel MapToModel(ChunkQueueMessageDto dto) => dto == null
          ? new ChunkDataModel()
          : new ChunkDataModel
          {
              Name = dto.FileName,
              ChunkIndex = dto.ChunkIndex,
              ChunksCount = dto.ChunksCount,
              StartPoint = dto.StartPoint,
              EndPoint = dto.EndPoint
          };
    }
}
