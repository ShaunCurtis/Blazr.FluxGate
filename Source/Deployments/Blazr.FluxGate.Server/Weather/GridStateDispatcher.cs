/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate.Server;

public readonly record struct UpdateGridPaging(int StartIndex, int PageSize) : IFluxGateAction;

public class GridStateDispatcher : FluxGateDispatcher<GridState>
{
    public override GridState Dispatch(GridState state, IFluxGateAction action)
    {
        return action switch
        {
            UpdateGridPaging a1 => Mutate(state, a1),
            _ => throw new NotImplementedException($"No Mutation defined for {action.GetType()}")
        };
    }

    private static GridState Mutate(GridState state, UpdateGridPaging action)
        => state with { StartIndex = action.StartIndex, PageSize = action.PageSize };
}
