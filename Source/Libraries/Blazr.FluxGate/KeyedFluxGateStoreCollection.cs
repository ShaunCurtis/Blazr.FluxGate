/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.FluxGate;

public class KeyedFluxGateStoreCollection<TState, TKey>
    where TState : new()
    where TKey : notnull
{
    private readonly FluxGateDispatcher<TState> _dispatcher;
    private readonly IServiceProvider _serviceProvider;

    private Dictionary<TKey, KeyedFluxGateStore<TState>> _items = new();

    public KeyedFluxGateStoreCollection(IServiceProvider serviceProvider, FluxGateDispatcher<TState> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
        _serviceProvider = serviceProvider;
    }

    public IFluxGateStore<TState>? GetItem(TKey key)
    {
        if (_items.TryGetValue(key, out KeyedFluxGateStore<TState>? store))
            return store;

        return default;
    }

    public bool AddStore(TKey key, TState state)
    {
        if (_items.ContainsKey(key))
            return false;

        var store = ActivatorUtilities.CreateInstance(_serviceProvider, typeof(KeyedFluxGateStore<TState>), new[] { state });
        if (store is null)
            return false;

        _items.Add(key, (KeyedFluxGateStore<TState>)store);
        return true;
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

    public IFluxGateStore<TState> GetOrCreateStore(TKey key)
    {
        KeyedFluxGateStore<TState>? store;

        if (_items.TryGetValue(key, out store))
            return store;

        store = ActivatorUtilities.CreateInstance<KeyedFluxGateStore<TState>>(_serviceProvider);

        ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TState).Name}.");

        _items.Add(key, store);

        return store;
    }

    public IFluxGateStore<TState> GetOrCreateStore(TKey key, TState initialState)
    {
        KeyedFluxGateStore<TState>? store;

        if (_items.TryGetValue(key, out store))
            return store;

        ArgumentNullException.ThrowIfNull(initialState);

        store = ActivatorUtilities.CreateInstance<KeyedFluxGateStore<TState>>(_serviceProvider, initialState);

        ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TState).Name}.");

        _items.Add(key, store);

        return store;
    }

    public void Dispatch(TKey key, IFluxGateAction action)
    {
        var store = this.GetOrCreateStore(key);
        store.Dispatch(action);
    }
}
