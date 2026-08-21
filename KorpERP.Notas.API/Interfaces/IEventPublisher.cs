namespace KorpERP.Notas.API.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent evento, CancellationToken cancellationToken = default) where TEvent : class;
    }
}