using Domain.Abstractions.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.Queries.Matches.ListRoles;

public class ListRolesQueryHandler : IRequestHandler<ListRolesQuery, List<ListRolesQueryResponse>>
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