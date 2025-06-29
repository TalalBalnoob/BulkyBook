using System.Linq.Expressions;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db){
        _db = db;
        dbSet = _db.Set<T>();
    }

    public IEnumerable<T> GetAll(string? propList = null, Expression<Func<T, bool>>? filter = null){
        IQueryable<T> query = dbSet;
        if (filter != null){
            query = query.Where(filter);
        }
        if (propList != null){
            foreach (var prop in propList.Split(new char[','], StringSplitOptions.RemoveEmptyEntries)){
                query = query.Include(prop);
            }
        }
        return query.ToList();
    }

    public T Get(Expression<Func<T, bool>> filter, string? propList = null, bool tracked = false){
        IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();
        query = dbSet.Where(filter);
        if (propList != null){
            foreach (var prop in propList.Split(new char[','], StringSplitOptions.RemoveEmptyEntries)){
                query = query.Include(prop);
            }
        }

        return query.FirstOrDefault();
    }

    public void Add(T entity){
        dbSet.Add(entity);
    }

    public void Delete(T entity){
        dbSet.Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities){
        dbSet.RemoveRange(entities);
    }
}