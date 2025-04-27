using DataAccess;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSignalR(cfg =>
{
    if (builder.Environment.IsDevelopment())
    {
        cfg.EnableDetailedErrors = true;
    }
});

builder.Services.InjectFirebaseAuth();
builder.Services.InjectUseCases();

FirebaseApp.Create(new AppOptions
{
    Credential = builder.Configuration.GetValue<string>("Firebase:UseJson") == "True"
                 ? GoogleCredential.FromFile(builder.Configuration.GetValue<string>("Firebase:CredentialPath"))
                 : GoogleCredential.FromJson(builder.Configuration.GetValue<string>("Firebase:CredentialJson"))
});

builder.Services.AddHttpClient<HttpClient>(cfg =>
{
    cfg.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
    {
        jwt.Authority = builder.Configuration.GetValue<string>("Firebase:Auth:TokenAuthorityUrl");
        jwt.Audience = builder.Configuration.GetValue<string>("Firebase:Auth:ProjectId");
        jwt.TokenValidationParameters.ValidIssuer = builder.Configuration.GetValue<string>("Firebase:Auth:ValidIssuerUrl");
        
        
        jwt.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (ctx.Request.Cookies.TryGetValue("Authorization", out var token))
                {
                    ctx.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddLogging(logging =>
{
    logging.AddApplicationInsights();
});

builder.Services.InjectDbContext(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetValue<string>("Cors:Origins") ?? throw new Exception("Cors:Origins")) // Adjust based on frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MatchHub>("/matchHub");

await app.RunAsync();