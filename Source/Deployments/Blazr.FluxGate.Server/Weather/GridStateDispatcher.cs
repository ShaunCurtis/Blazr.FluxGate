/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate.Server;

public readonly record struct UpdateGridPaging(object Sender, int StartIndex, int PageSize) : IFluxGateAction;

public class GridStateDispatcher : FluxGateDispatcher<GridState>
{
    public override FluxGateResult<GridState> Dispatch(FluxGateStore< GridState> store, IFluxGateAction action)
    {
        return action switch
        {
            UpdateGridPaging a1 => Mutate(store, a1),
            _ => throw new NotImplementedException($"No Mutation defined for {action.GetType()}")
        };
    }

    private static FluxGateResult<GridState> Mutate(FluxGateStore<GridState> store, UpdateGridPaging action)
    { 
        var newItem =  store.Item with { StartIndex = action.StartIndex, PageSize = action.PageSize };
        var state = newItem != store.Item ? store.State.Modified() : store.State;

        return new FluxGateResult<GridState>(true, newItem, state);    
    }
}
