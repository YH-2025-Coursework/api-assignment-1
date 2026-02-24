using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Workshop.Api.Data;
using Workshop.Api.Options;
using Workshop.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind JWT configuration so authentication middleware and the token endpoint share the same settings.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration section 'Jwt' is missing.");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("Jwt:Key must be configured.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorization();

// Registers AppDbContext with the DI container so it can be injected wherever it's requested (e.g., into WorkshopService).
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Pull the Default connection string from config/user secrets.
    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' not found. Ensure appsettings or user secrets contain it.");

    // Use SQL Server.
    options.UseSqlServer(connectionString);
});

// Register workshop and session domain services so each request gets a scoped instance via DI.
builder.Services.AddScoped<IWorkshopService, WorkshopService>();
builder.Services.AddScoped<ISessionService, SessionService>();

// Register MVC controllers so API endpoints can be mapped via attributes.

/*
MVC stands for Model–View–Controller, the architectural pattern ASP.NET Core uses for its controller-based web stack.
Even when we’re building APIs (no views), the framework still calls the controller package “MVC”, so AddControllers()
wires up that MVC infrastructure - model binding, validation, filters, and attribute routing - without the Razor view pieces.
*/

builder.Services.AddControllers();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

// Route attribute-routed controllers (e.g., WorkshopsController).

/*
app.MapControllers() tells ASP.NET Core to scan the app for controller classes and hook up their attribute routes
(like [Route("api/workshops")] and [HttpGet]). Once that’s called, any HTTP request matching those attributes gets
dispatched to the corresponding controller action; without it, the routing table wouldn’t include our controllers, so
their endpoints wouldn’t respond.
*/

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
