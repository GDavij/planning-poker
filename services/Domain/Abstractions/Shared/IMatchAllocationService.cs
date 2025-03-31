using Domain.Entities;

namespace Domain.Abstractions.Shared;


public interface IMatchAllocationService
{
    bool HasMatchAllocated(Match match);
}