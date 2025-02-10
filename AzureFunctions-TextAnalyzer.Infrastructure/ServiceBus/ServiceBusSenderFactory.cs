using Azure.Messaging.ServiceBus;

namespace AzureFunctions_TextAnalyzer.Infrastructure.ServiceBus
{
    public class ServiceBusSenderFactory : IServiceBusSenderFactory
    {
        private readonly string _connectionString;

        public ServiceBusSenderFactory(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public ServiceBusSender CreateSender(string queueName)
        {
            return new ServiceBusClient(_connectionString).CreateSender(queueName);
        }
    }
}