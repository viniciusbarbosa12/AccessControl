using Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Domain.Config;

namespace Data.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> CreateAndSaveAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // SOFT DELETE
        public virtual async Task DeleteAsync(T entity)
        {
            entity.MarkAsDeleted();
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking().Where(x => x.DeletedAt == null);
        }

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            return _dbSet.AsNoTracking().Where(x => x.DeletedAt == null).Where(expression);
        }


        public virtual async Task<PaginatedResult<T>> GetAllPagedAsync<PagedFilter>(
            PagedQuery<PagedFilter> query,
            Func<IQueryable<T>, PagedFilter?, IQueryable<T>>? filterLogic = null)
        {
            query.Normalize();

            var dbQuery = _dbSet.AsNoTracking().Where(x => x.DeletedAt == null);

            if (filterLogic != null)
                dbQuery = filterLogic(dbQuery, query.Filter);

            var totalItems = await dbQuery.CountAsync();
            var items = await dbQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize),
                Page = query.Page,
                PageSize = query.PageSize
            };
        }



        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity?.DeletedAt != null)
                return null;

            return entity;
        }

        public virtual async Task<T?> GetOneAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.DeletedAt == null)
                .FirstOrDefaultAsync(expression);
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual T Update(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }
    }
}
