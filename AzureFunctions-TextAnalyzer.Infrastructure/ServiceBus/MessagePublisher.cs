using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace AzureFunctions_TextAnalyzer.Infrastructure.ServiceBus
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IServiceBusSenderFactory _senderFactory;
        private readonly ILogger<MessagePublisher> _logger;

        public MessagePublisher(IServiceBusSenderFactory senderFactory, ILogger<MessagePublisher> logger)
        {
            _senderFactory = senderFactory;
            _logger = logger;
        }

        public async Task PublishMessageAsync(string message, string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentException("Queue name cannot be null or empty.", nameof(queueName));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            }

            var sender = _senderFactory.CreateSender(queueName);

            try
            {
                var serviceBusMessage = new ServiceBusMessage(message);

                _logger.LogInformation("Sending message to queue: {QueueName}", queueName);

                await sender.SendMessageAsync(serviceBusMessage);

                _logger.LogInformation("Message sent successfully to queue: {QueueName}", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message to queue: {QueueName}. Message: {Message}", queueName, message);
                throw;
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }
    }
}