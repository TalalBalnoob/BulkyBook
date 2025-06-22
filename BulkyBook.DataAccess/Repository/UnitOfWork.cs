using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork{
    private ApplicationDbContext _db;
    public ICategoryRepository category{ get; private set; }
    public IProductRepository product{ get; private set; }

    public UnitOfWork(ApplicationDbContext db){
        _db = db;
        category = new CategoryRepository(_db);
        product = new ProductRepository(_db);
    }

    public void Save(){
        _db.SaveChanges();
    }
}