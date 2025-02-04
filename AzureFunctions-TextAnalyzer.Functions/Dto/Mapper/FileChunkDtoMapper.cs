using AzureFunctions_TextAnalyzer.Service.Model;


namespace AzureFunctions_TextAnalyzer.Functions.Dto.Mapper
{
    public class FileChunkDtoMapper : IDtoMapper<FileChunkModel, FileChunkDto>
    {
        public FileChunkDto MapFromModel(FileChunkModel model) => model == null
          ? new FileChunkDto()
          : new FileChunkDto
          {
              PartitionKey = model.PartitionKey,
              FileName = model.RowKey,
              Status = model.Status
          };

        public FileChunkModel MapToModel(FileChunkDto dto) => dto == null
          ? new FileChunkModel()
          : new FileChunkModel
          {
              PartitionKey = dto.PartitionKey,
              RowKey = dto.FileName,
              Status = dto.Status,
          };
    }
}
