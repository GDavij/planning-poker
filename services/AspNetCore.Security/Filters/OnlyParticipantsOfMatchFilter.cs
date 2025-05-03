using System.Net;
using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Abstractions.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Security.Filters;

public class OnlyParticipantsOfMatchFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var currentAccount = context.HttpContext.RequestServices.GetRequiredService<ICurrentAccount>();
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<IApplicationDbContext>();
        
        var matchIdString = context.RouteData.Values["matchId"]?.ToString();

        if (long.TryParse(matchIdString, out long matchId) &&
            await dbContext.Matches.Include(m => m.Participants)
                                    .AnyAsync(m => m.MatchId == matchId &&
                                                         m.Participants.Any(p => p.AccountId == currentAccount.AccountId)))
        {
            await next();
            return;
        }

        context.Result = new UnauthorizedObjectResult(new Notification("You are not allowed to use this action", "Stories.CannotListFromMatch", HttpStatusCode.Unauthorized));
    }
}