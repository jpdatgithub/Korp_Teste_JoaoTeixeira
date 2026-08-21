using MassTransit;
using KorpERP.Notas.API.Interfaces;

namespace KorpERP.Notas.API.Infrastructure
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