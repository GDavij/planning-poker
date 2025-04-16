using Domain.Abstractions.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace WebApi.UseCases.Queries.Matches.ListRoles;

public class ListRolesQueryHandler
{
    private readonly IApplicationDbContext _dbContext;

    public ListRolesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ListRolesQueryResponse>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        return _dbContext.Roles.OrderBy(r => r.Name)
                               .Select(r => new ListRolesQueryResponse(r.RoleId, r.Name, r.Abbreviation))
                               .ToListAsync(cancellationToken);
    }
}