# Weather Grid Example

This example demonstrates maintaining paging state in multiple Blazor QuickGrids.

It uses the `KeyedFluxGateStore`.  The key for each grid is a page based static Guid. 

### FluxGate Objects

First a base GridState object to hold the pertinent grid data.  We'll be interfacing with QuickGrid's `GridItemsProviderRequest`, but we don't want to use that object because it's too specific.

```csharp
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
```

A single action:

```csharp
public readonly record struct UpdateGridPaging(int StartIndex, int PageSize) : IFluxGateAction;
```

And the dispatcher and mutation:

```csharp
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
```

And the DI registration:

```csharp
builder.Services.AddScoped<KeyedFluxGateStore<GridState, Guid>>();
builder.Services.AddSingleton<FluxGateDispatcher<GridState>, GridStateDispatcher>();
```

### WeatherForecast Provider

A simple DB emulator. It implements `GetItemsAsync` as a QuickGrid `GridItemsProvider` delegate pattern.  

```csharp
public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```
```csharp
public class WeatherProvider
{
    private int _recordCount = 500;
    private readonly List<WeatherForecast> _weather;

    public WeatherProvider()
    {
        _weather = GetData();
    }

    public async ValueTask<GridItemsProviderResult<WeatherForecast>> GetItemsAsync(GridItemsProviderRequest<WeatherForecast> request)
    {
        await Task.Yield();

        var query = _weather
            .Skip(request.StartIndex)
            .Take(request.Count ?? 10);

        return new GridItemsProviderResult<WeatherForecast>() { Items = query.ToList(), TotalItemCount = _weather.Count() };
    }

    protected List<WeatherForecast> GetData()
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
        return Enumerable.Range(1, _recordCount).Select(index => new WeatherForecast
        {
            Date = startDate.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        }).ToList();
    }
}
```

DI registration:

```csharp
builder.Services.AddSingleton<WeatherProvider>();
```

### Weather Page

First the properties/fields:

```csharp
    [Parameter] public int DefaultPageSize { get; set; } = 10;

    private static Guid PageUid = Guid.NewGuid();
    private QuickGrid<WeatherForecast>? _quickGrid;
    private PaginationState _pagination = new();
```

`PageUid` provides the Key.  It's static, so it will always be the same for this SPA session.

Injected Services:

```csharp
@inject WeatherProvider Provider
@inject KeyedFluxGateStore<GridState, Guid> KeyedStore
```

`OnInitializedAsync` gets the existing state (if it exists) and applies it to the `PaginationState`.

```csharp
protected override async Task OnInitializedAsync()
{
    var store = KeyedStore.GetOrCreateStore(PageUid, new GridState(0, this.DefaultPageSize));

    // Set the initial pagination
    _pagination.ItemsPerPage = store.Item.PageSize;
    await _pagination.SetCurrentPageIndexAsync(store.Item.Page);
}
```

The *GridItemsProvider*.  It dispatches the action to the keyed store before passing the async awaitable to the caller. 

```csharp
private ValueTask<GridItemsProviderResult<WeatherForecast>> GetItemsAsync(GridItemsProviderRequest<WeatherForecast> request)
{
    this.KeyedStore.Dispatch(PageUid, new UpdateGridPaging(StartIndex: request.StartIndex, PageSize: request.Count ?? this.DefaultPageSize));
    return Provider.GetItemsAsync(request);
}
```

The Markup:

```csharp
<div class="grid">

    <QuickGrid TGridItem="WeatherForecast" ItemsProvider="this.GetItemsAsync" Pagination="_pagination" @ref="_quickGrid">
        <PropertyColumn Title="Date" Property="(item) => item.Date" />
        <PropertyColumn Title="Temperature &deg;C" Property="(item) => item.TemperatureC" />
        <PropertyColumn Title="Temperature &deg;F" Property="(item) => item.TemperatureF" />
        <PropertyColumn Title="Summary" Property="(item) => item.Summary" />
    </QuickGrid>

</div>
```

You now have a working example.

Add another copy of the Weather page to the application and add it to `NavMenu`.  Go to each page and move to different pages.  You will see that the appication maintains the individual state of the two grids.

