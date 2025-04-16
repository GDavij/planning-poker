namespace WebApi.UseCases.Commands.Matches.ApproveJoinRequest;

public record ApproveJoinRequestCommand(
    bool HasApproved,
    string RequesterConnectionId,
    long RequesterAccountId,
    long MatchId,
    string ApproverConnectionId);
    