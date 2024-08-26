using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;
using Persistence.Models;

namespace Persistence.Repositories;

public class TableSpecificationRepository : GenericRepository<TableSpecification>, ITableSpecificationRepository
{
    private readonly TableContext _context;
    public TableSpecificationRepository(TableContext context) : base(context)
    {
        _context = context;

    }
    public async Task AddAsync(TableSpecification tableSpecification)
    {
         _context.TableSpecifications.Add(tableSpecification);
        await _context.SaveChangesAsync();
    }
    public async Task<TableSpecification> GetByIdAsync(Guid tableId)
    {
        return await _context.TableSpecifications.FindAsync(tableId);
    }

    public async Task UpdateAsync(TableSpecification tableSpecification)
    {
        _context.TableSpecifications.Update(tableSpecification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TableSpecification tableSpecification)
    {
        _context.TableSpecifications.Remove(tableSpecification);
        await _context.SaveChangesAsync();
    }
}