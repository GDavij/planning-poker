using Application;
using AspnetCore.Observability;
using AspNetCore.Presentation.Factories;
using AspNetCore.Security;
using DataAccess;
using Firebase.Auth;
using Scalar.AspNetCore;
using WebApi;
using WebApi.Ports.SignalR;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    // Clear default configuration sources
    builder.Configuration.Sources.Clear();

    // Add environment variables as the only configuration source
    builder.Configuration.AddEnvironmentVariables();
}

builder.Services.AddApplicationInsightsTelemetryAndLogging()
                .AddErrorHandlerMiddleware();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = ValidationResponseFactory.HandleInvalidModelState;
});

builder.Services.AddSignalR(cfg =>
{
    if (builder.Environment.IsDevelopment())
    {
        cfg.EnableDetailedErrors = true;
    }
});

builder.Services.AddFirebaseAuth(builder.Configuration)
                .AddFirebaseJwtValidation(builder.Configuration);

builder.Services.AddRateLimitingForClients()
                .AddCorsSetup(builder.Configuration);

builder.Services.AddApplication()
                .AddNotification()
                .AddSignalRServiceClients()
                .AddSignalRIntegrationClients();


builder.Services.AddDataAccess(builder.Configuration);

builder.Services.AddHttpClient<HttpClient>(cfg =>
{
    cfg.Timeout = TimeSpan.FromMinutes(5);
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}




app.UseErrorHandlerMiddleware();
app.UseRequestLogging();

app.UseCustomRateLimiting();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MatchHub>("/matchHub");

app.MapGet("/", () => $"Running over Environment {app.Environment.EnvironmentName}");

await app.RunAsync();