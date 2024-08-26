using Persistence.DatabaseContext;
using Persistence.Models;

namespace Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly TableContext _context;

    public GenericRepository(TableContext context)
    {
        _context = context;
    }

    public List<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }

    public T? GetById(Guid id)
    {
        return _context.Set<T>().Find(id);
    }

    public async Task<TableSpecification> GetByIdAsync(Guid tableId)
    {
        return await _context.TableSpecifications.FindAsync(tableId);
    }

    public void Remove(T itemToRemove)
    {
        _context.Set<T>().Remove(itemToRemove);
    }
}