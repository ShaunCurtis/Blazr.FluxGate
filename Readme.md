#  Blazr.FluxGate

*Blazr.FluxGate* is a succinct *no frills* DotNetCore implementation of the *Flux Pattern*.

It implements these primary Flux pattern requirements:

1. The state object is immutable.
1. Mutations are defined in pure methods.
1. Mutation occurs by passing an action to a Dispatcher.

In addition it implements the concept of the keyed store that maintains multiple individual stores of a specific type with a unique key.

You can see two examples:

1. [Simple Counter Implementation](Counter-Example.md)
1. [Weather QuickGrid Paging State Implementation](Weather-Example.md)

Two commentaries on the implementations:

1. [FluxGateStore and support classes](FluxGate.md)
1. [KeyedFluxGateStore](KeyedFluxGate.md)
