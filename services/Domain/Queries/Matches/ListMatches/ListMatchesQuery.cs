using MediatR;

namespace Domain.Queries.Matches.ListMatches;

public record ListMatchesQuery(int Page, byte Limit) : IRequest<List<ListMatchesQueryResponse>>;