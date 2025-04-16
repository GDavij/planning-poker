namespace WebApi.UseCases.Commands.Matches.CreateMatch;

public record CreateMatchCommand(string Description, byte RoleId, bool ShouldSpectate);