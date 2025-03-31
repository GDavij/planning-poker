using DataAccess;
using Domain;
using Firebase.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Ports.SignalR;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.InjectUseCases();
builder.Services.InjectFirebaseAuth();

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(builder.Configuration.GetValue<string>("Firebase:CredentialPath"))
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