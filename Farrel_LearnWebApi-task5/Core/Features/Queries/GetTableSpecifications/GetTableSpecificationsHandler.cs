using MediatR;
using Persistence.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace Core.Features.Queries.GetTableSpecifications;

public class GetTableSpecificationsHandler : IRequestHandler<GetTableSpecificationsQuery, GetTableSpecificationsResponse>
{
    private readonly ITableSpecificationRepository _tableSpecificationRepository;
    private readonly IConnectionMultiplexer _redis;

    public GetTableSpecificationsHandler(ITableSpecificationRepository tableSpecificationRepository, IConnectionMultiplexer redis)
    {
        _tableSpecificationRepository = tableSpecificationRepository;
        _redis = redis;
    }

    public async Task<GetTableSpecificationsResponse> Handle(GetTableSpecificationsQuery query, CancellationToken cancellationToken)
    {
        var db = _redis.GetDatabase();
        string cacheKey = $"TableSpecification_{query.TableSpecificationId}";

        try
        {
            // Coba ambil data dari Redis
            string cachedData = await db.StringGetAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<GetTableSpecificationsResponse>(cachedData);
            }
        }
        catch (RedisConnectionException)
        {
            // Log Redis error 
            Console.WriteLine("Redis tidak tersedia, melanjutkan ke SQL");
        }

        // Jika data tidak ada di Redis atau Redis gagal, ambil dari SQL
        var tableSpecification = await _tableSpecificationRepository.GetByIdAsync(query.TableSpecificationId);

        if (tableSpecification is null)
            return new GetTableSpecificationsResponse();

        var response = new GetTableSpecificationsResponse
        {
            TableId = tableSpecification.TableId,
            ChairNumber = tableSpecification.ChairNumber,
            TableNumber = tableSpecification.TableNumber,
            TablePic = tableSpecification.TablePic,
            TableType = tableSpecification.TableType
        };

        // Simpan data ke Redis (jika Redis tersedia kembali)
        try
        {
            await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(response), TimeSpan.FromMinutes(10));
        }
        catch (RedisConnectionException)
        {
            // Redis masih tidak tersedia, abaikan dan teruskan
            Console.WriteLine("Gagal menyimpan ke Redis, melanjutkan tanpa cache");
        }

        return response;
    }
}
