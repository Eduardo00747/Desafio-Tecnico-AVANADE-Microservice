

namespace Vendas.API.Services
{
    public interface IRabbitMQService
    {
        void PublishOrderCreated(object payload);
    }
}