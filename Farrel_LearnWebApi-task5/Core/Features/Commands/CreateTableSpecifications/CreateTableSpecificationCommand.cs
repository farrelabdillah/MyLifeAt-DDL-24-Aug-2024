using MediatR;

namespace Core.Features.Commands.CreateTableSpecification
{
    public class CreateTableSpecificationCommand : IRequest<CreateTableSpecificationResponse>
    {
        public int TableNumber { get; set; }
        public int ChairNumber { get; set; }
        public string TablePic { get; set; }
        public string TableType { get; set; }
    }
}
