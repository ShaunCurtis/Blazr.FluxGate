/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate;

//public delegate TState FluxDispatherDelegate<TState>(TState inState, IFluxAction action);

public class FluxGateStore<TState> : IFluxGateStore<TState>
    where TState : IFluxGateState<TState>, new()
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
