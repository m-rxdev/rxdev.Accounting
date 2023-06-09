using Microsoft.EntityFrameworkCore;
using rxdev.Accounting.Model;
using System.Linq.Expressions;

namespace rxdev.Accounting.Persistence;

public class Repository<T>
    where T : Entity, new()
{
    protected readonly DbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public Repository(AccountingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
    }

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Count()
        => _dbSet.Count();

    public void Count(Expression<Func<T, bool>> predicate)
        => _dbSet.Count(predicate);

    public T Get(int id, bool tracking = false)
        => AsQueryable(tracking).Single(x => x.Id == id);

    public IQueryable<T> AsQueryable(bool tracking = false)
        => tracking 
        ? _dbSet.AsTracking()
        : _dbSet.AsNoTracking();

    public void Add(T entity)
        => _dbSet.Add(entity);

    public void AddRange(IEnumerable<T> entities)
        => _dbSet.AddRange(entities);

    public void Remove(T entity)
        => _dbSet.Remove(entity);

    public void Remove(int id)
    {
        T entity = new() { Id = id };
        _dbContext.Entry(entity).State = EntityState.Deleted;
    }
}