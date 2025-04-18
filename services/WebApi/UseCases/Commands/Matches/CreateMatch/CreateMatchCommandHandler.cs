using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApi.UseCases.Commands.Matches.CreateMatch;

public class CreateMatchCommandHandler 
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentAccount _currentAccount;
    private readonly INotificationService _notificationService;

    public CreateMatchCommandHandler(IApplicationDbContext dbContext, ICurrentAccount currentAccount, INotificationService notificationService)
    {
        _dbContext = dbContext;
        _currentAccount = currentAccount;
        _notificationService = notificationService;
    }

    public async Task<CreateMatchCommandResponse?> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == request.RoleId);
        if (role is null)
        {
            _notificationService.AddNotification("Role could not be found", "Roles.NotFound");
            return null;
        }
        
        var match = new Match(_currentAccount, request.Description);
        var participant = new Participant(_currentAccount, role);
        
        if (request.ShouldSpectate)
        {
            participant.SpectateMatch();
        }

        participant.Join(match, _notificationService);
        
        _dbContext.Matches.Add(match);

        
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        
        return new CreateMatchCommandResponse(match.MatchId);
    }
}