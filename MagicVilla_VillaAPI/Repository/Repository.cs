using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;
      
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.dbSet = _context.Set<T>();
        }
        public async Task Create(T entity)
        {
            await dbSet.AddAsync(entity);
            await Save();
        }
        //"villa,villa special"
        public async Task<T> Get(Expression<Func<T, bool>>? filter = null, bool tracker = true, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if(!tracker) query = query.AsNoTracking();
            if (filter != null) query = query.Where(filter);
         
            if (includeProperties != null) {
                foreach (var includeProperty in includeProperties.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(includeProperty);                  
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 3, int pageNumber = 1)
        {
          
            IQueryable<T> query = dbSet;
            if (filter != null) query = query.Where(filter);
            if (pageSize > 0)
            {
                if (pageSize > 100) pageSize = 100;
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }
            if (includeProperties!=null) {
                foreach (var includeProperty in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)) { 
                    query =query.Include(includeProperty);
                }
            }
            return await query.ToListAsync();
        }

        public async Task Remove(T entity)
        {
            dbSet.Remove(entity);
            await Save();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
        
    }
}
