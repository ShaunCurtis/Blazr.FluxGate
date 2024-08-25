/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.FluxGate.Server;

public class WeatherForecast
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

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
