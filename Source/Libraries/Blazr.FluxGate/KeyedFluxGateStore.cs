/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.FluxGate;

public class KeyedFluxGateStore<TFluxGateItem, TKey>
    where TFluxGateItem : new()
    where TKey : notnull
{
    private readonly FluxGateDispatcher<TFluxGateItem> _dispatcher;
    private readonly IServiceProvider _serviceProvider;

    private Dictionary<TKey, FluxGateStore<TFluxGateItem>> _items = new();

    public KeyedFluxGateStore(IServiceProvider serviceProvider, FluxGateDispatcher<TFluxGateItem> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
        _serviceProvider = serviceProvider;
    }

    public FluxGateStore<TFluxGateItem>? GetStore(TKey key)
    {
        if (_items.TryGetValue(key, out FluxGateStore<TFluxGateItem>? store))
            return store;

        return default;
    }

    public FluxGateStore<TFluxGateItem> GetOrCreateStore(TKey key)
    {
        FluxGateStore<TFluxGateItem>? store;

        if (_items.TryGetValue(key, out store))
            return store;

        store = (FluxGateStore<TFluxGateItem>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<TFluxGateItem>));

        ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TFluxGateItem).Name}.");

        _items.Add(key, store);

        return store;
    }

    public FluxGateStore<TFluxGateItem> GetOrCreateStore(TKey key, TFluxGateItem initialState)
    {
        FluxGateStore<TFluxGateItem>? store;

        if (_items.TryGetValue(key, out store))
            return store;

        ArgumentNullException.ThrowIfNull(initialState);

        store = (FluxGateStore<TFluxGateItem>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<TFluxGateItem>), initialState);

        ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TFluxGateItem).Name}.");

        _items.Add(key, store);

        return store;
    }

    public bool RemoveStore(TKey key)
    {
        if (_items.ContainsKey(key))
        {
            _items.Remove(key);
            return true;
        }
        return false;
    }

    public void Dispatch(TKey key, IFluxGateAction action)
    {
        var store = this.GetOrCreateStore(key);
        store.Dispatch(action);
    }
}
