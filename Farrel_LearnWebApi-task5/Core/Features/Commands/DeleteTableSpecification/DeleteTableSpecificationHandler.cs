using MediatR;
using Persistence.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features.Commands.DeleteTableSpecification
{
    public class DeleteTableSpecificationHandler : IRequestHandler<DeleteTableSpecificationCommand, DeleteTableSpecificationResponse>
    {
        private readonly ITableSpecificationRepository _tableSpecificationRepository;

        public DeleteTableSpecificationHandler(ITableSpecificationRepository tableSpecificationRepository)
        {
            _tableSpecificationRepository = tableSpecificationRepository;
        }

        public async Task<DeleteTableSpecificationResponse> Handle(DeleteTableSpecificationCommand command, CancellationToken cancellationToken)
        {
            var tableSpecification = await _tableSpecificationRepository.GetByIdAsync(command.TableId);

            if (tableSpecification == null)
            {
                return new DeleteTableSpecificationResponse
                {
                    Success = false,
                    Message = "Table specification not found."
                };
            }

            await _tableSpecificationRepository.DeleteAsync(tableSpecification);

            return new DeleteTableSpecificationResponse
            {
                Success = true,
                Message = "Table specification deleted successfully."
            };
        }
    }
}
