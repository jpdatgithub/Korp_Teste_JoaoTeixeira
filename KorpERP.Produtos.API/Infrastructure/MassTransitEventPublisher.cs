using MassTransit;
using KorpERP.Produtos.API.Interfaces;

namespace KorpERP.Produtos.API.Infrastructure
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<TEvent>(TEvent evento, CancellationToken cancellationToken = default) where TEvent : class
        {
            await _publishEndpoint.Publish(evento, cancellationToken);
        }
    }
}