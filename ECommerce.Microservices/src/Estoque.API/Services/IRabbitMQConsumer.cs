using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estoque.API.Services
{
    public interface IRabbitMQConsumer
    {
        void Start();
    }
}