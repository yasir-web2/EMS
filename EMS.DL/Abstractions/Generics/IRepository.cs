using EMS.Entities;
using System.Linq.Expressions;

namespace EMS.DL.Abstractions.Generics;

public interface IRepository<T> where T : EntityBase
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddWithSaveAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    Task UpdateWithSaveAsync(T entity, CancellationToken cancellationToken = default);
    void Delete(T entity);
    Task DeleteWithSaveAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default);
    Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[] includes, CancellationToken cancellationToken = default);
    IQueryable<T> AsQueryable();
    IQueryable<T> AsQueryable(Expression<Func<T, bool>> predicate);
}