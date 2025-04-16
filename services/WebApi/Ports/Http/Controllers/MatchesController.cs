using Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.UseCases.Commands.Matches.CreateMatch;
using WebApi.UseCases.Queries.Matches.ListMatches;
using WebApi.UseCases.Queries.Matches.ListRoles;

namespace WebApi.Ports.Http.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public MatchesController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartNewMatch([FromBody] CreateMatchCommand command,
        [FromServices] CreateMatchCommandHandler handler)
    {
        var result = await handler.Handle(command, CancellationToken.None);

        return Created("/matches", result);
    }

    [HttpGet]
    public async Task<IActionResult> ListUserCreatedMatches([FromQuery] ListMatchesQuery query,
        [FromServices] ListMatchesQueryHandler handler, CancellationToken cancellationToken)
    {
        return Ok(await handler.Handle(query, cancellationToken));
    }

    [HttpGet("roles")]
    public async Task<IActionResult> ListValidRoles([FromQuery] ListRolesQuery query,
        [FromServices] ListRolesQueryHandler handler, CancellationToken cancellationToken)
    {
        return Ok(await handler.Handle(query, cancellationToken));
    }
}