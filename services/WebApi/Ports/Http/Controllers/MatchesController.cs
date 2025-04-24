using Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using WebApi.UseCases.Commands.Matches.CreateMatch;
using WebApi.UseCases.Commands.Stories.AddStory;
using WebApi.UseCases.Commands.Stories.DeleteStory;
using WebApi.UseCases.Commands.Stories.SelectStory;
using WebApi.UseCases.Commands.Stories.UpdateStory;
using WebApi.UseCases.Commands.Stories.VoteStory;
using WebApi.UseCases.Queries.Matches.ListMatches;
using WebApi.UseCases.Queries.Matches.ListParticipants;
using WebApi.UseCases.Queries.Matches.ListRoles;
using WebApi.UseCases.Queries.Stories;
using WebApi.UseCases.Queries.Stories.ListStories;

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

    [HttpGet("match/{matchId}/stories")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> ListStories(
        [FromRoute] long matchId,
        [FromServices] ListStoriesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(matchId, cancellationToken);

        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }

        return Ok(result);
    }

    [HttpGet("match/{matchId}/participants")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> ListParticipantsOfMatch(
        [FromRoute] long matchId,
        [FromServices] ListParticipantsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(matchId, cancellationToken);
        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }

        return Ok(result);
    }

    [HttpPost("match/{matchId}/story/add")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> AddStoryToMatch(
        [FromRoute] long matchId,
        [FromBody] AddStoryCommand command,
        [FromServices] AddStoryCommandHandler handler)
    {
        await handler.Handle(matchId, command);
        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }
        
        return Created();
    }

    [HttpPut("match/{matchId}/story/{storyId}/update")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> UpdateStoryOfMatch(
        [FromRoute] long matchId,
        [FromRoute] long storyId,
        [FromBody] UpdateStoryCommand command,
        [FromServices] UpdateStoryCommandHandler handler)
    {
        await handler.Handle(matchId, storyId, command);
        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }

        return Accepted();
    }

    [HttpDelete("match/{matchId}/story/{storyId}")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> DeleteStoryOfMatch(
        [FromRoute] long matchId,
        [FromRoute] long storyId,
        [FromServices] DeleteStoryCommandHandler handler)
    {
        await handler.Handle(matchId, storyId);
        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }

        return Accepted();
    }

    [HttpPatch("match/{matchId}/story/{storyId}")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> SelectStoryToShowUp(
        [FromRoute] long matchId,
        [FromRoute] long storyId,
        [FromServices] SelectStoryCommandHandler handler)
    {
        await handler.Handle(matchId, storyId);
        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }

        return Ok();
    }

    [HttpPatch("match/{matchId}/story/{storyId}/vote/{points}")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> Vote(
        [FromRoute] long matchId,
        [FromRoute] long storyId,
        [FromRoute] short points,
        [FromServices] VoteStoryCommandHandler handler)
    {
        await handler.Handle(matchId, storyId, points);
        if (_notificationService.HasNotifications())
        {
            return BadRequest(_notificationService.GetNotifications());
        }

        return Accepted();
    }

}