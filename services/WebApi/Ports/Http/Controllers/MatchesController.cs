using System.Net;
using Application.UseCases.Planning.Matches.CloseMatch;
using Application.UseCases.Planning.Matches.CreateMatch;
using Application.UseCases.Planning.Matches.ListMatches;
using Application.UseCases.Planning.Matches.ListParticipants;
using Application.UseCases.Planning.Matches.ListRoles;
using Application.UseCases.Planning.Stories.AddStory;
using Application.UseCases.Planning.Stories.DeleteStory;
using Application.UseCases.Planning.Stories.ListStories;
using Application.UseCases.Planning.Stories.SelectStory;
using Application.UseCases.Planning.Stories.UpdateStory;
using Application.UseCases.Planning.Stories.VoteStory;
using AspNetCore.Security.Filters;
using Domain.Abstractions;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Ports.Http.Controllers;

[Authorize]
public class MatchesController : BaseApiController
{

    public MatchesController(
        INotificationService notificationService,
        IHttpContextAccessor httpContextAccessor,
        TelemetryClient telemetryClient) 
        : base(notificationService, httpContextAccessor, telemetryClient)
    { }

    [HttpPost("start")]
    public async Task<IActionResult> StartNewMatch([FromBody] CreateMatchCommand command,
        [FromServices] CreateMatchCommandHandler handler)
    {
        var result = await handler.Handle(command);

        return RespondWith(result, HttpStatusCode.Created);
    }

    [HttpGet]
    public async Task<IActionResult> ListUserCreatedMatches([FromQuery] ListMatchesQuery query,
        [FromServices] ListMatchesQueryHandler handler, CancellationToken cancellationToken)
    {
        return RespondWith(await handler.Handle(query, cancellationToken), HttpStatusCode.OK);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> ListValidRoles([FromServices] ListRolesQueryHandler handler, CancellationToken cancellationToken)
    {
        return RespondWith(await handler.Handle(cancellationToken), HttpStatusCode.OK);
    }

    [HttpGet("match/{matchId}/stories")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> ListStories(
        [FromRoute] long matchId,
        [FromServices] ListStoriesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(matchId, cancellationToken);

        return RespondWith(result, HttpStatusCode.OK);
    }

    [HttpGet("match/{matchId}/participants")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> ListParticipantsOfMatch(
        [FromRoute] long matchId,
        [FromServices] ListParticipantsQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(matchId, cancellationToken);
        return RespondWith(result, HttpStatusCode.OK);
    }

    [HttpPost("match/{matchId}/story/add")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> AddStoryToMatch(
        [FromRoute] long matchId,
        [FromBody] AddStoryCommand command,
        [FromServices] AddStoryCommandHandler handler)
    {
        return RespondWith(await handler.Handle(matchId, command), HttpStatusCode.Created);
    }

    [HttpPut("match/{matchId}/story/{storyId}/update")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> UpdateStoryOfMatch(
        [FromRoute] long matchId,
        [FromRoute] long storyId,
        [FromBody] UpdateStoryCommand command,
        [FromServices] UpdateStoryCommandHandler handler)
    {
        return RespondWith(await handler.Handle(matchId, storyId, command), HttpStatusCode.Accepted);
    }

    [HttpDelete("match/{matchId}/story/{storyId}")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> DeleteStoryOfMatch(
        [FromRoute] long matchId,
        [FromRoute] long storyId,
        [FromServices] DeleteStoryCommandHandler handler)
    {
        
        return RespondWith(await handler.Handle(matchId, storyId), HttpStatusCode.Accepted);
    }

    [HttpPatch("match/{matchId}/story/{storyId}")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> SelectStoryToShowUp(
        [FromRoute] long matchId,
        [FromRoute] long storyId,
        [FromServices] SelectStoryCommandHandler handler)
    {
        return RespondWith(await handler.Handle(matchId, storyId), HttpStatusCode.Accepted);
    }

    [HttpPatch("match/{matchId}/story/{storyId}/vote/{points}")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> Vote(
        [FromRoute] long matchId,
        [FromRoute] long storyId,
        [FromRoute] short points,
        [FromServices] VoteStoryCommandHandler handler)
    {
        return RespondWith(await handler.Handle(matchId, storyId, points), HttpStatusCode.Accepted);
    }

    [HttpPatch("match/{matchId}/finish")]
    [OnlyParticipantsOfMatchFilter]
    public async Task<IActionResult> FinishMatch(
        [FromRoute] long matchId,
        [FromServices] CloseMatchCommandHandler handler)
    {
        return RespondWith(await handler.Handle(matchId), HttpStatusCode.Accepted);
    }
}