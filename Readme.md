#  Blazr.FluxGate

*Blazr.FluxGate* is a simple implementation of the *Flux Pattern*.

To demonstrate using the `Counter` page:

We define a State object:

```csharp
public record CounterState
{
    public int Counter { get; init; }
}
```

Two actions to apply to the state:

```csharp
public record CounterIncrementAction(int IncrementBy) : IFluxGateAction;
public record CounterDecrementAction(int DecrementBy) : IFluxGateAction;
```

And a Dispatcher to apply the actions:

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

We add the following two services to DI:

```csharp
builder.Services.AddScoped<IFluxGateStore<CounterState>, FluxGateStore<CounterState>>();
builder.Services.AddSingleton<FluxGateDispatcher<CounterState>, CounterStateDispatcher>();
```

And use them in `Counter`:

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