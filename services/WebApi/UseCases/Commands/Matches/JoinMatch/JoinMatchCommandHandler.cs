using Domain.Abstractions;
using Domain.Abstractions.Auth.Models;
using Domain.Abstractions.DataAccess;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApi.Ports.SignalR;

namespace WebApi.UseCases.Commands.Matches.JoinMatch;

public class JoinMatchCommandHandler
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentAccount _currentAccount;
    private readonly IHubContext<MatchHub> _matchHubContext;

    public JoinMatchCommandHandler(IApplicationDbContext dbContext, ICurrentAccount currentAccount, IHubContext<MatchHub> matchHubContext)
    {
        _dbContext = dbContext;
        _currentAccount = currentAccount;
        _matchHubContext = matchHubContext;
    }

    public async Task<JoinMatchCommandResponse?> Handle(JoinMatchCommand request, CancellationToken cancellationToken)
    {
        /* Rascunho dos passos a serem implementados
         *
         * 1. Puxar a partida e o participante, se a partida existir e o participante não
         *      1.1. -> Chamar uma chamada para o grupo da partida validar permitir o acesso (Criar outro endpoint para adicionar o usuário na partida(Passar o id e a connection id do usuário)
         *      1.2. Caso o usuário existir  -> Aprovar entrada no sistema sem perguntar (adicionar a connection id a o grupo de usuários da partida)
         */
        
        var match = await _dbContext.Matches.Include(m => m.Participants.Where(p => p.AccountId == _currentAccount.AccountId))
                                            .FirstOrDefaultAsync(m => m.MatchId == request.MatchId, cancellationToken);

        if (match is null)
        {
            await _matchHubContext.Clients.Client(request.ConnectionId)
                .SendAsync("ReceiveErrorAsync", new Notification("Could not find match.", "Matches.NotFound"));
            return null;
        }
        
        var participant = match.Participants.FirstOrDefault();

        if (participant is null)
        {
            await _matchHubContext.Clients.Group(match.MatchId.ToString())
                .SendAsync("AskAdminForApprovalForMatchAsync", request.ConnectionId, _currentAccount.AccountId);

            return new JoinMatchCommandResponse();
        }


        participant.ConnectedAt(request.ConnectionId);
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        await _matchHubContext.Clients.Client(request.ConnectionId).SendAsync("ApproveJoinRequest", CancellationToken.None);
        
        return new JoinMatchCommandResponse();
    }
}