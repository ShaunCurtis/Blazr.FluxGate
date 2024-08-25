# Implementing the Flux Pattern

There key requirements that define the Flux pattern are:

1. The state object is immutable.
1. Mutations are defined in pure methods.
1. Mutation occurs by passing an action to a Dispatcher.

The CounterState is defined as a record with all properties set to `init`.  If default values are required thay should be set in a parameterless constructor.

A record is an immutable reference type with value-based equality.

```csharp
public record CounterState
{
    public int Counter { get; init; }
}
```

Next the store:

```csharp
public class FluxGateStore<TState> 
    where TState : new()
{
    private readonly FluxGateDispatcher<TState> _dispatcher;

    public TState Item { get; private set; } = new();
    public event EventHandler<FluxGateEventArgs>? StateChanged;

    public FluxGateStore(FluxGateDispatcher<TState> fluxStateDispatcher)
    {
        _dispatcher = fluxStateDispatcher;
    }

    public void Dispatch(IFluxGateAction action)
    {
        this.Item = _dispatcher.Dispatch(this.Item, action);

        this.StateChanged?.Invoke(action, new FluxGateEventArgs() { State=this.Item });
    }
}
```

An interface to identify Actions:

```csharp
public interface IFluxGateAction { }
```

And an abstract `FluxGateDispatcher` base implementation:

```csharp
public abstract class FluxGateDispatcher<TState>
{
    public abstract TState Dispatch(TState state, IFluxGateAction action);
}
```

Finally an `EventArgs` implementation for the `StateChanged` event:

```csharp
public class FluxGateEventArgs : EventArgs
{
    public object? State { get; init; }
}
```

That's it.  Now an implementation.

The Counter state object.  An immutable `record`.

```csharp
public record CounterState
{
    public int Counter { get; init; }
}
```

Two actions:

```csharp
public readonly record struct CounterIncrementAction(int IncrementBy) : IFluxGateAction;
public readonly record struct CounterDecrementAction(int DecrementBy) : IFluxGateAction;
```

And the Dispatcher:

```csharp
public class CounterStateDispatcher: FluxGateDispatcher<CounterState>
{
    public override CounterState Dispatch(CounterState state, IFluxGateAction action)
    {
        return action switch
        {
            CounterIncrementAction a1 => Mutate(state, a1),
            CounterDecrementAction a2 => Mutate(state, a2),
            _ => throw new NotImplementedException($"No Mutation defined for {action.GetType()}")
        };
    }

    private static CounterState Mutate(CounterState state, CounterIncrementAction action)
        => state with { Counter = state.Counter + action.IncrementBy };

    public static CounterState Mutate(CounterState state, CounterDecrementAction action)
        => state with { Counter = state.Counter - action.DecrementBy };
}
```

The reducer methods are in the dispatcher class, and are private, so only usable by the dispatcher.  They can be in separate static classes, but that exposes their functionality to the rest of the application. 