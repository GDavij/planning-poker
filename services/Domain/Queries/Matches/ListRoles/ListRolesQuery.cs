using MediatR;

namespace Domain.Queries.Matches.ListRoles;

public record ListRolesQuery : IRequest<List<ListRolesQueryResponse>>;
    