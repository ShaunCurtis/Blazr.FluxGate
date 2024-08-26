# FluxGate

The generic `TFluxGateItem` represents the tracked object.  

The only constraint applied is `where TFluxGateItem : new()`.

## The Supporting Objects

`FluxGateState` tracks the state of the store.  It's a struct so a value object: a new instance is created every time it's assigned.  It's initial state is either *New* or *Existing*.

```csharp
public struct FluxGateState
{
    private bool _isNew;
    private bool _isModified;
    private bool _isDeleted;

    public readonly bool IsNew => _isNew;
    public bool IsDeleted => _isDeleted;
    public bool IsModified => _isModified;

    public FluxGateState() { }

    public FluxGateState Modified(bool value = true)
        => new FluxGateState() { _isNew = this.IsNew, _isDeleted = this.IsDeleted, _isModified = value };

    public FluxGateState Deleted(bool value = true)
        => new FluxGateState() { _isNew = this.IsNew, _isDeleted = value, _isModified = this.IsModified };

    public static FluxGateState AsNew() => new FluxGateState() { _isNew = true };
    public static FluxGateState AsExisting() => new FluxGateState();
}
```

`IFluxGateAction` identifies Actions:

```csharp
public interface IFluxGateAction { }
```

`FluxGateResult` is a result value object returned by the dispatcher.  It returns the mutated `TFluxGateItem` and the new `FluxGateState`.

```csharp
public readonly record struct FluxGateResult<TFluxGateItem>(TFluxGateItem Item, FluxGateState State);
```

`FluxGateDispatcher` is a base abstract implementation of the Dispatcher.  `Dispatch` takes a store and action as arguments and returns a `FluxGateResult` containing the mutated item and a new state.

```csharp
public abstract class FluxGateDispatcher<TItem>
    where TItem : new()
{
    public abstract FluxGateResult<TItem> Dispatch(FluxGateStore<TItem> store, IFluxGateAction action);
}
```

`FluxGateEventArgs` is an `EventArgs` implementation for the `StateChanged` event:

```csharp
public class FluxGateEventArgs : EventArgs
{
    public object? State { get; init; }
}
```

### FluxGateStore

`FluxGateStore` provides a wrapper around a `TFluxGateItem` instance.

It has three public properties:

 - `Item` is the current tracked `TFluxGateItem`.  Don't assign it to local variables: you will have a reference to a stale object if it mutates.
 - `State` is the current state of the store. Note `FluxGateState` is a value object.
 - `StateChanged` is an Event raised whenever the `TFluxGateItem` instance is mutated. 

`_dispatcher` holds a reference to the `FluxGateDispatcher` registered against `TFluxGateItem`.

```csharp
public class FluxGateStore<TFluxGateItem>
    where TFluxGateItem : new()
{
    private readonly FluxGateDispatcher<TFluxGateItem> _dispatcher;

    public TFluxGateItem Item { get; private set; }
    public FluxGateState State { get; private set; } = FluxGateState.AsNew();
    public event EventHandler<FluxGateEventArgs>? StateChanged;
```
Two Constructors:
 - The first is the DI constructor that just gets the DI registered `FluxGateDispatcher<TFluxGateItem>`.  It initializes the store with a **New** `TFluxGateItem`.
 - The second takes an additional `TFluxGateItem` argument.  It's used by `ActivatorUtilities` in `KeyedFluxGateStore` to create store instances outside DI control. 

```csharp
    public FluxGateStore(FluxGateDispatcher<TFluxGateItem> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
        this.Item = new();
    }

    public FluxGateStore(FluxGateDispatcher<TFluxGateItem> fluxStateDispatcher, TFluxGateItem state)
    {
        _dispatcher = fluxStateDispatcher;
        this.Item = state;
        this.State = FluxGateState.AsExisting();
    }
```

`Dispatch` dispatches the provided `IFluxGateAction` to the registered `FluxGateDispatcher`.  It applies the returned `Item` and `State` to the store.

```csharp
    public void Dispatch(IFluxGateAction action)
    {
        var result = _dispatcher.Dispatch(this, action);
        this.Item = result.Item;
        this.State = result.State;

        this.StateChanged?.Invoke(action, new FluxGateEventArgs() { State = this.Item });
    }
}
```

That's it.  

[You can see an implementation here.](Counter-Example.md)
