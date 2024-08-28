#  Counter Example

Define an immutable state object:

```csharp
public record CounterState
{
    public int Counter { get; init; }
}
```

Two actions to apply mutations to the state:

```csharp
public record CounterIncrementAction(int IncrementBy) : IFluxGateAction;
public record CounterDecrementAction(int DecrementBy) : IFluxGateAction;
```

A Dispatcher to apply the actions:

```csharp
public class CounterStateDispatcher: FluxGateDispatcher<CounterState>
{
    public override FluxGateResult<CounterState> Dispatch(FluxGateStore<CounterState> store, IFluxGateAction action)
    {
        return action switch
        {
            CounterIncrementAction a1 => Mutate(store, a1),
            CounterDecrementAction a2 => Mutate(store, a2),
            _ => throw new NotImplementedException($"No Mutation defined for {action.GetType()}")
        };
    }

    private static FluxGateResult<CounterState> Mutate(FluxGateStore<CounterState> store, CounterIncrementAction action)
    {
        var state = store.State.Modified();
        var newItem = store.Item with { Counter = store.Item.Counter + action.IncrementBy };

        return new(newItem, state);
    }

    public static FluxGateResult<CounterState> Mutate(FluxGateStore<CounterState> store, CounterDecrementAction action)
    {
        var state = store.State.Modified();
        var newItem = store.Item with { Counter = store.Item.Counter - action.DecrementBy };

        return new(newItem, state);
    }
}
```

And two service definitions in DI:

```csharp
builder.Services.AddScoped<IFluxGateStore<CounterState>, FluxGateStore<CounterState>>();
builder.Services.AddSingleton<FluxGateDispatcher<CounterState>, CounterStateDispatcher>();
```

Update `Counter`:

```csharp
@page "/counter"

@inject IFluxGateStore<CounterState> Store

<PageTitle>Counter</PageTitle>

<h1>Counter</h1>

<p role="status">Current count: @this.Store.Item.Counter</p>

<button class="btn btn-success" @onclick="IncrementCount">Increment</button>
<button class="btn btn-danger" @onclick="() => DecrementCount(1)">Decrement by 1</button>
<button class="btn btn-danger" @onclick="() => DecrementCount(2)">Decrement by 2</button>

@code {

    private void IncrementCount()
    {
        this.Store.Dispatch(new CounterIncrementAction(1));
    }

    private void IncrementCount(int by)
    {
        this.Store.Dispatch(new CounterIncrementAction(by));
    }
    
    private void DecrementCount(int by)
    {
        this.Store.Dispatch(new CounterDecrementAction(by));
    }
}
```

And you'll see Blazr FluxGate in action.