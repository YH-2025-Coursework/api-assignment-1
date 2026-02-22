namespace Workshop.Api.Options;

// Binds the Jwt configuration section so controllers and middleware share the same values.
public sealed class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string DemoPassword { get; set; } = string.Empty;
}
