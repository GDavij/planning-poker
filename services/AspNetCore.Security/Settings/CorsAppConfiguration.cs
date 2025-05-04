namespace AspNetCore.Security.Settings;

public class CorsSettings
{
    public required string Origins { get; set; }
    public required string[] Headers { get; set; }
    public required string[] Methods { get; set; }
}
