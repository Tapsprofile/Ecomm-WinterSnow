using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Data.Repositories;

public class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly WinterSnowDbContext _db;

    public EfRepository(WinterSnowDbContext db)
    {
        _db = db;
    }

    public IQueryable<TEntity> Table => _db.Set<TEntity>().AsQueryable();

    public Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
    {
        IQueryable<TEntity> query = _db.Set<TEntity>();
        if (predicate is not null)
            query = query.Where(predicate);
        return await query.ToListAsync(ct);
    }

    public async Task InsertAsync(TEntity entity, CancellationToken ct = default)
    {
        await _db.Set<TEntity>().AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        _db.Set<TEntity>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken ct = default)
    {
        _db.Set<TEntity>().Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}

