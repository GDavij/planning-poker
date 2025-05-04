using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Planning.Matches.ListRoles;

public record ListRolesQueryResponse(byte RoleId, string Name, string? Abbreviation);

public class ListRolesQueryHandler
{
    private readonly IApplicationDbContext _dbContext;

    public ListRolesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ListRolesQueryResponse>> Handle(CancellationToken cancellationToken)
    {
        return _dbContext.Roles.OrderBy(r => r.Name)
                               .Select(r => new ListRolesQueryResponse(r.RoleId, r.Name, r.Abbreviation))
                               .ToListAsync(cancellationToken);
    }
}