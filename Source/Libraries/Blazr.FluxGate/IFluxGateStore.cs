/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate;

public interface IFluxGateStore<TState>
    where TState : IFluxGateState<TState>
{
    public TState Item { get; }
    public event EventHandler<FluxGateEventArgs>? StateChanged;

    public void Dispatch(IFluxGateAction action);
}
