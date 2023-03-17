using EMS.DL.Abstractions.Generics;
using EMS.DL.DataContext;
using EMS.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS.DL.Implementations.Generics;

public sealed class Repository<T> : IRepository<T> where T : EntityBase
{
    #region Private Fields

    private readonly EMSDbContext _dbContext;

    #endregion

    #region Constructor

    public Repository(EMSDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion

    #region Public Methods

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Entity().AddAsync(entity, cancellationToken);
    }

    public async Task AddWithSaveAsync(T entity, CancellationToken cancellationToken = default)
    {
        await AddAsync(entity, cancellationToken);
        await SaveAsync(cancellationToken);
    }

    public void Update(T entity)
    {
        Entity().Update(entity);
    }

    public async Task UpdateWithSaveAsync(T entity, CancellationToken cancellationToken = default)
    {
        Update(entity);
        await SaveAsync(cancellationToken);
    }

    public void Delete(T entity)
    {
        Entity().Remove(entity);
    }

    public async Task DeleteWithSaveAsync(T entity, CancellationToken cancellationToken = default)
    {
        Delete(entity);
        await SaveAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
    {
        return await Entity().FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Entity().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = AsQueryable(predicate);
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Entity().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Query().Where(predicate);
        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[] includes, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = Query().Where(predicate);
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public IQueryable<T> AsQueryable()
    {
        return Query().AsNoTracking();
    }

    public IQueryable<T> AsQueryable(Expression<Func<T, bool>> predicate)
    {
        return Query().Where(predicate).AsNoTracking();
    }

    #endregion


    #region Private Helpers

    private async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<T> Query()
    {
        return Entity().AsQueryable();
    }

    private DbSet<T> Entity()
    {
        return _dbContext.Set<T>();
    }

    #endregion
}