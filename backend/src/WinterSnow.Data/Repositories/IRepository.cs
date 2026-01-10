using System.Linq.Expressions;
using WinterSnow.Core.Domain.Common;

namespace WinterSnow.Data.Repositories;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    IQueryable<TEntity> Table { get; }

    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

    Task InsertAsync(TEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TEntity entity, CancellationToken ct = default);
}

