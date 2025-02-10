namespace AzureFunctions_TextAnalyzer.Infrastructure.ServiceBus
{
    public interface IMessagePublisher
    {
        Task PublishMessageAsync(string message, string queueName);
    }
}
