using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Vendas.API.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMQService> _logger;

        public RabbitMQService(IConnectionFactory factory, ILogger<RabbitMQService> logger)
        {
            _logger = logger;
            _connection = factory.CreateConnection();
        }

        public void PublishOrderCreated(object payload)
        {
            using var channel = _connection.CreateModel();
            var queueName = "order.created";
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
            _logger.LogInformation("Published order.created event");
        }
    }
}