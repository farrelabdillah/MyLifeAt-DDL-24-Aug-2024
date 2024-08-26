using MediatR;
using Persistence.Models;
using Persistence.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace Core.Features.Commands.CreateTableSpecification
{
    public class CreateTableSpecificationHandler : IRequestHandler<CreateTableSpecificationCommand, CreateTableSpecificationResponse>
    {
        private readonly ITableSpecificationRepository _tableSpecificationRepository;
        private readonly IConnectionMultiplexer _redis;

        public CreateTableSpecificationHandler(ITableSpecificationRepository tableSpecificationRepository, IConnectionMultiplexer redis)
        {
            _tableSpecificationRepository = tableSpecificationRepository;
            _redis = redis;
        }

        public async Task<CreateTableSpecificationResponse> Handle(CreateTableSpecificationCommand command, CancellationToken cancellationToken)
        {
            var newTableSpecification = new TableSpecification
            {
                TableId = Guid.NewGuid(),
                TableNumber = command.TableNumber,
                ChairNumber = command.ChairNumber,
                TablePic = command.TablePic,
                TableType = command.TableType
            };

            await _tableSpecificationRepository.AddAsync(newTableSpecification);

            // Sinkronisasi data ke Redis
            var db = _redis.GetDatabase();
            string cacheKey = $"TableSpecification_{newTableSpecification.TableId}";

            var cacheData = new CreateTableSpecificationResponse
            {
                TableId = newTableSpecification.TableId,
                Success = true,
                Message = "Table specification created successfully."
            };

            // Simpan ke Redis dengan TTL 10 menit
            await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(cacheData), TimeSpan.FromMinutes(10));

            return cacheData;
        }
    }
}
