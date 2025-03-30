using Domain.Abstractions;
using Domain.Commands.Accounts.CreateAccount;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Exception = System.Exception;

namespace WebApi.Ports.Http.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly INotificationService _notificationService;

    public AuthController(IMediator mediator, INotificationService notificationService)
    {
        _mediator = mediator;
        _notificationService = notificationService;
    }

    [HttpPost("save-session")]
    [AllowAnonymous]
    public async Task<IActionResult> SaveSession([FromBody] TokenRequest tokenRequest)
    {
        try
        {
            var token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(tokenRequest.OAuthToken);

            var cookieOptions = new CookieOptions
            {
                Domain = "localhost",
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
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand createAccountCommand)
    {
        var result = await _mediator.Send(createAccountCommand);

        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }

        return Created("/accounts", result);
    }

    [HttpPost("autologin")]
    public async Task<IActionResult> CreateGoogleAccount()
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

            var result = await _mediator.Send(createAccountCommand);

            if (_notificationService.HasNotifications())
            {
                return BadRequest(_notificationService.GetNotifications());
            }

            return Ok(result);
        }
        catch (Exception exception)
        {
            return Unauthorized();
        }
    }

}
public record TokenRequest(string OAuthToken);
