# Keyed FluxGate

The standard implementations of Flux are built for single store instances.

Let's consider the the user case where we want to maintain the state of several grid forms.  Specifically for this demonstration, when we return to a form, we come back to the same page we were on when we left.

The example implementation is described here [Weather Example](Weather-Example.md)

The Keyed FluxGate Store provides this functionality.  It provides a wrapper around a key/store dictionary.

The class wireframe looks like this.

```csharp
public class KeyedFluxGateStore<TState, TKey>
    where TState : new()
    where TKey : notnull
{
    private readonly FluxGateDispatcher<TState> _dispatcher;
    private readonly IServiceProvider _serviceProvider;

    private Dictionary<TKey, FluxGateStore<TState>> _items = new();

    public KeyedFluxGateStore(IServiceProvider serviceProvider, FluxGateDispatcher<TState> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
        _serviceProvider = serviceProvider;
    }

    public FluxGateStore<TState>? GetStore(TKey key);
    public FluxGateStore<TState> GetOrCreateStore(TKey key);
    public FluxGateStore<TState> GetOrCreateStore(TKey key, TState initialState);

    public bool RemoveStore(TKey key);

    public void Dispatch(TKey key, IFluxGateAction action);
}
```

Most of the functionality is self evident.

### GetOrCreateStore  

There are two versions: one which creates a default `TState` if it doesn't exist and one that creates a store using the provided `TState` instance.

```csharp
public FluxGateStore<TState> GetOrCreateStore(TKey key, TState initialState)
{
```

First we try and test for an existing store in the dictionary.  if one exists, we return it.

```csharp
    FluxGateStore<TState>? store;

    if (_items.TryGetValue(key, out store))
        return store;
```

After testing we don't have null `initialState`, we use `ActivatorUtilities` to create an instance of `FluxGateStore<TState>` from the DI ServiceProvider.

```csharp
    ArgumentNullException.ThrowIfNull(initialState);

    store = (FluxGateStore<TState>)ActivatorUtilities.CreateInstance(_serviceProvider, typeof(FluxGateStore<TState>), initialState);
```

  It's important to understand that the instance we create is not managed by the DI ServiceProvider.  We are just using it to provide the service instances to populate the constructor.

  In this case we provide a `TState` instance as an additional parameter.  `ActivatorUtilities` will create a `FluxGateStore` instance using this  constructor.  `FluxGateDispatcher<TState>` will come from the DI container.

```csharp
public FluxGateStore(FluxGateDispatcher<TState> fluxStateDispatcher, TState state)
```

Finally we check to a null value - the activator failed, add the store to the dictionary and return the store reference.

```csharp
    ArgumentNullException.ThrowIfNull(store, $"No store defined in DI for {typeof(TState).Name}.");

    _items.Add(key, store);

    return store;
}
```
