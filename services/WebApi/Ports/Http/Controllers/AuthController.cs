using System.Net;
using Application.UseCases.Management.Accounts.CreateAccount;
using Application.UseCases.Management.Accounts.Me;
using Domain.Abstractions;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Ports.Http.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AuthController : BaseApiController
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        INotificationService notificationService,
        IConfiguration configuration,
        ILogger<AuthController> logger,
        IHttpContextAccessor httpContextAcessor,
        TelemetryClient telemetryClient) : base(notificationService, httpContextAcessor, telemetryClient)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("save-session")]
    [AllowAnonymous]
    public async Task<IActionResult> SaveSession([FromBody] TokenRequest tokenRequest)
    {
        try
        {
            var token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenRequest.OAuthToken);

            var domain = _configuration.GetValue<string>("API_DOMAIN") ?? "localhost";
            _logger.LogInformation("Got Current Domain as: {domain}", domain);
            
            var cookieOptions = new CookieOptions
            {
                Domain = domain,
                Expires = DateTimeOffset.FromUnixTimeSeconds(token.ExpirationTimeSeconds),
                Path = "/",
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None
            };
            
            Response.Cookies.Append("Authorization", tokenRequest.OAuthToken, cookieOptions);
        }
        catch (FirebaseException exception)
        {
            return Unauthorized("Invalid token");
        }

        return Accepted();
    }

    [HttpPost("new-login")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand createAccountCommand, [FromServices] CreateAccountCommandHandler handler)
    {
        var result = await handler.Handle(createAccountCommand, CancellationToken.None);

        return RespondWith(result, HttpStatusCode.Created);
    }

    [HttpPost("autologin")]
    public async Task<IActionResult> CreateGoogleAccount([FromServices] CreateAccountCommandHandler handler)
    {
        try
        {
            //TODO: Need To Get Custom Claims
            var createAccountCommand = new CreateAccountCommand(
               User.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value,
               User.Claims.First(c => c.Type == "name").Value,
               null)
            {
                FirebaseUserId = User.Claims.First(c => c.Type == "user_id").Value,
                PhotoUrl = User.Claims.First(c => c.Type == "picture").Value
            };

            var result = await handler.Handle(createAccountCommand, CancellationToken.None);

            return RespondWith(result, HttpStatusCode.OK);
        }
        catch (Exception exception)
        {
            _logger.LogError("Catch Exception when Trying to Authenticate Endpoint: {exception}", exception);
            return Unauthorized();
        }
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(GetMeQueryHandler handler)
    {
        return RespondWith(await handler.Handle(), HttpStatusCode.OK);
    }
}
public record TokenRequest(string OAuthToken);
