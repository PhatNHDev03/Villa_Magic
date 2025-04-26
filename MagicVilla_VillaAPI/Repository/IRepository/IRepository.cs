using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties =null,
            int pageSize =3 , int pageNumber = 1);
        Task<T> Get(Expression<Func<T, bool>>? filter = null, bool tracker = true, string? includeProperties = null);

        Task Create(T entity);
     
        Task Save();
        Task Remove(T entity);
    }
}
