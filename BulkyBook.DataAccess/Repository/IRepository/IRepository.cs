using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IRepository<T> where T : class{
    IEnumerable<T> GetAll(string? propList = null);
    T Get(Expression<Func<T, bool>> filter, string? propList = null, bool tracked = false);
    void Add(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
}