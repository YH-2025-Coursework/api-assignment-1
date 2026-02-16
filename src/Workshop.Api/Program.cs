/*
Kestrel is the built-in web server used by ASP.NET Core.

- It listens on a network port
- Receives HTTP requests
- Sends HTTP responses

Kestrel is wired in automatically by ASP.NET Core.
It “lives” inside the hosting setup created by the next line:
*/

var builder = WebApplication.CreateBuilder(args);

/*
Under the hood, that:
- Creates a generic host
- Configures Kestrel as the web server
- Applies launchSettings.json / environment settings
- Builds the request pipeline
*/

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// app.Run() starts Kestrel and makes it begin listening on the configured ports.
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
