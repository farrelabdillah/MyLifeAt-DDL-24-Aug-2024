using MediatR;

namespace Core.Features.Queries.GetTableSpecifications;

public class GetTableSpecificationsQuery : IRequest<GetTableSpecificationsResponse>
{
    public Guid TableSpecificationId { get; set; }
}