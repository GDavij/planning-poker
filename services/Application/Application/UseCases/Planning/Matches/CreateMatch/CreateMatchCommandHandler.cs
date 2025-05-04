using System.ComponentModel.DataAnnotations;
using System.Net;
using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Matches.CreateMatch;

public record CreateMatchCommand
{
    [Required(ErrorMessage = "Description is Required")]
    [MaxLength(120, ErrorMessage = "Description must have a max of 120 characters")]
    public required string Description { get; init; }
    
    [Required(ErrorMessage = "Role Id is required")]
    [Range(1, byte.MaxValue, ErrorMessage = "Role is not valid")]
    public byte RoleId { get; init; }
    
    [Required(ErrorMessage = "You need to say if you gonna enter the match as a spectator or not")]
    public bool ShouldSpectate { get; init; }
}

public record CreateMatchCommandResponse(long MatchId);

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

    public async Task<CreateMatchCommandResponse?> Handle(CreateMatchCommand request)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == request.RoleId);
        if (role is null)
        {
            _notificationService.AddNotification("Role could not be found", "Roles.NotFound", HttpStatusCode.NotFound);
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
        
        await _dbContext.SaveChangesAsync();
        
        return new CreateMatchCommandResponse(match.MatchId);
    }
}