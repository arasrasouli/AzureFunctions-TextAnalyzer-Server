using Azure.Messaging.ServiceBus;

namespace AzureFunctions_TextAnalyzer.Infrastructure.ServiceBus
{
    public interface IServiceBusSenderFactory
    {
        ServiceBusSender CreateSender(string queueName);
    }
}
