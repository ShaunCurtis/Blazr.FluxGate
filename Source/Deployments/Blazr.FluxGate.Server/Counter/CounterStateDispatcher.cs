/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate.Server;

public readonly record struct CounterIncrementAction(object Sender, int IncrementBy) : IFluxGateAction;
public readonly record struct CounterDecrementAction(object Sender, int DecrementBy) : IFluxGateAction;

public class CounterStateDispatcher : FluxGateDispatcher<CounterState>
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

        return new(true, newItem, state);
    }

    public static FluxGateResult<CounterState> Mutate(FluxGateStore<CounterState> store, CounterDecrementAction action)
    {
        var state = store.State.Modified();
        var newItem = store.Item with { Counter = store.Item.Counter - action.DecrementBy };

        return new(true,newItem, state);
    }
}
