/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate.Server;

public record GridState
{
    public int StartIndex { get; init; }
    public int PageSize { get; init; }
    public int Page => this.StartIndex / this.PageSize;

    public GridState()
    {
        this.StartIndex = 0;
        this.PageSize = 10;
    }

    public GridState(int startIndex, int pageSize)
    {
        StartIndex = startIndex;
        PageSize = pageSize;
    }

    public static GridState FromGridRequest<T>(GridItemsProviderRequest<T> request)
    {
        return new(request.StartIndex, request.Count ?? 1000);
    }
}

