using Domain.Abstractions;
using Domain.Commands.Matches.CreateMatch;
using Domain.Commands.Matches.TakePartOfMatch;
using Domain.Queries.Matches.ListMatches;
using Domain.Queries.Matches.ListRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Ports.Http.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly INotificationService _notificationService;

    public MatchesController(IMediator mediator, INotificationService notificationService)
    {
        _mediator = mediator;
        _notificationService = notificationService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartNewMatch([FromBody] CreateMatchCommand command)
    {
        var result = await _mediator.Send(command);

        return Created("/matches", result);
    }

    [HttpPatch("take-part/{matchId}")]
    public async Task<IActionResult> TakePartOfAMatch([FromRoute] long matchId, [FromBody] TakePartOfMatchCommand command)
    {
        command = command with
        {
            MatchId = matchId
        };

        await _mediator.Send(command);

        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }

        return Accepted();
    }

    [HttpGet]
    public async Task<IActionResult> ListUserCreatedMatches([FromQuery] ListMatchesQuery query)
    {
        return Ok(await _mediator.Send(query));
    }

    [HttpGet("roles")]
    public async Task<IActionResult> ListValidRoles()
    {
        return Ok(await _mediator.Send(new ListRolesQuery()));
    }
    
}