using MediatR;
using Persistence.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Features.Commands.UpdateTableSpecification
{
    public class UpdateTableSpecificationHandler : IRequestHandler<UpdateTableSpecificationCommand, UpdateTableSpecificationResponse>
    {
        private readonly ITableSpecificationRepository _tableSpecificationRepository;

        public UpdateTableSpecificationHandler(ITableSpecificationRepository tableSpecificationRepository)
        {
            _tableSpecificationRepository = tableSpecificationRepository;
        }

        public async Task<UpdateTableSpecificationResponse> Handle(UpdateTableSpecificationCommand command, CancellationToken cancellationToken)
        {
            var tableSpecification = await _tableSpecificationRepository.GetByIdAsync(command.TableId);

            if (tableSpecification == null)
            {
                return new UpdateTableSpecificationResponse
                {
                    Success = false,
                    Message = "Table specification not found."
                };
            }

            tableSpecification.TableNumber = command.TableNumber;
            tableSpecification.ChairNumber = command.ChairNumber;
            tableSpecification.TablePic = command.TablePic;
            tableSpecification.TableType = command.TableType;

            await _tableSpecificationRepository.UpdateAsync(tableSpecification);

            return new UpdateTableSpecificationResponse
            {
                Success = true,
                Message = "Table specification updated successfully."
            };
        }
    }
}
