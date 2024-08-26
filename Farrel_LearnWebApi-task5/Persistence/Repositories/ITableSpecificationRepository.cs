using Persistence.Models;
using System.Threading.Tasks;
namespace Persistence.Repositories;

public interface ITableSpecificationRepository : IGenericRepository<TableSpecification>
{
    Task AddAsync(TableSpecification tableSpecification);
    Task<TableSpecification> GetByIdAsync(Guid tableId);
    Task UpdateAsync(TableSpecification tableSpecification);
    Task DeleteAsync(TableSpecification tableSpecification);
}