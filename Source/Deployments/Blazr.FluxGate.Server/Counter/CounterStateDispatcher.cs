namespace Blazr.FluxGate.Server;

public record CounterIncrementAction(int IncrementBy) : IFluxGateAction;
public record CounterDecrementAction(int DecrementBy) : IFluxGateAction;

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
