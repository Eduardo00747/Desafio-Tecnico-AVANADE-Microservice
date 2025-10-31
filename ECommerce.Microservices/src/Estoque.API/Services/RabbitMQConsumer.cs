using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Estoque.API.Repositories;

namespace Estoque.API.Services
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConsumer(IServiceScopeFactory serviceScopeFactory, ILogger<RabbitMQConsumer> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "order.created", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            _logger.LogInformation("✅ Conectado ao RabbitMQ e aguardando mensagens...");

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"📩 Mensagem recebida: {message}");

                try
                {
                    var orderData = JsonSerializer.Deserialize<OrderMessage>(message);
                    if (orderData != null && orderData.Items != null)
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var productRepo = scope.ServiceProvider.GetRequiredService<IProductRepository>();

                        foreach (var item in orderData.Items)
                        {
                            await productRepo.UpdateStockAsync(item.ProductId, item.Quantity);
                            _logger.LogInformation($"✅ Estoque atualizado para o produto {item.ProductId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Erro ao processar mensagem: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: "order.created", autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }

    public class OrderMessage
    {
        public int OrderId { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
