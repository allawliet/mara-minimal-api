using imas.ApiService.Application.Common;
using imas.ApiService.Domain.Common;

namespace imas.ApiService.Infrastructure.Services;

/// <summary>
/// Implementation of domain event dispatcher using dependency injection
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching domain event: {EventType}", domainEvent.GetType().Name);

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        var tasks = new List<Task>();

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod("HandleAsync");
            if (handleMethod != null)
            {
                var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
                tasks.Add(task);
            }
        }

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("Successfully dispatched domain event: {EventType}", domainEvent.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching domain event: {EventType}", domainEvent.GetType().Name);
            throw;
        }
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        var eventsList = domainEvents.ToList();
        
        if (!eventsList.Any())
        {
            return;
        }

        _logger.LogInformation("Dispatching {Count} domain events", eventsList.Count);

        var tasks = eventsList.Select(domainEvent => DispatchAsync(domainEvent, cancellationToken));

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("Successfully dispatched all {Count} domain events", eventsList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching domain events");
            throw;
        }
    }
}
