using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Entities;
using MediatR;

namespace Domain.Commands.Matches.CreateMatch;

public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, CreateMatchCommandResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentAccount _currentAccount;

    public CreateMatchCommandHandler(IApplicationDbContext dbContext, ICurrentAccount currentAccount)
    {
        _dbContext = dbContext;
        _currentAccount = currentAccount;
    }

    public async Task<CreateMatchCommandResponse> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        var match = new Match(_currentAccount, request.Description);

        _dbContext.Matches.Add(match);

        await _dbContext.SaveChangesAsync(CancellationToken.None);

        return new CreateMatchCommandResponse(match.MatchId);
    }
}