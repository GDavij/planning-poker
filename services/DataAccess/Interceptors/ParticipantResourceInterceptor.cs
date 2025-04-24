using Domain.Abstractions.Auth.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DataAccess.Interceptors;

internal class ParticipantResourceInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentAccount _currentAccount;
    private readonly long _matchId = 0;
    
    public ParticipantResourceInterceptor(ICurrentAccount currentAccount, IHttpContextAccessor httpContextAccessor)
    {
        _currentAccount = currentAccount;
        if (httpContextAccessor.HttpContext is not null)
        {
            var routeData = httpContextAccessor.HttpContext.GetRouteData();
            if (routeData is not null && routeData.Values.TryGetValue("matchId", out object value) && value is long matchId)
            {
                _matchId = matchId;
            }
        }
    }
    
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        HandleParticipantsResources(eventData);
        
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        HandleParticipantsResources(eventData);
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void HandleParticipantsResources(DbContextEventData eventData)
    {
        foreach (var resourceEntry in eventData.Context!.ChangeTracker.Entries<IParticipantResource>())
        {
            resourceEntry.Entity.MatchId = _matchId;
            resourceEntry.Entity.AccountId = _currentAccount.AccountId;
        }
    }
}