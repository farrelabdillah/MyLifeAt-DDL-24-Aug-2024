namespace Core.Features.Commands.CreateTableSpecification
{
    public class CreateTableSpecificationResponse
    {
        public Guid TableId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
