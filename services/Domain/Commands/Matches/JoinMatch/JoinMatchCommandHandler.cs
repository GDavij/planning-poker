using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Domain.Abstractions.Shared;
using MediatR;

namespace Domain.Commands.Matches.JoinMatch;

public class JoinMatchCommandHandler : IRequestHandler<JoinMatchCommand, JoinMatchCommandResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMatchAllocationService _matchAllocationService;
    private readonly ICurrentAccount _currentAccount;

    public JoinMatchCommandHandler(IApplicationDbContext dbContext, IMatchAllocationService matchAllocationService, ICurrentAccount currentAccount)
    {
        _dbContext = dbContext;
        _matchAllocationService = matchAllocationService;
        _currentAccount = currentAccount;
    }

    public Task<JoinMatchCommandResponse> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
    {
        // Should Treat Admin Scenarios
    }
}