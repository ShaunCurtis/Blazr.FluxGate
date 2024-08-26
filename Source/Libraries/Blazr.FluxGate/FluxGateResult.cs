/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate;

public readonly record struct FluxGateResult<TFluxGateItem>(TFluxGateItem Item, FluxGateState State);
