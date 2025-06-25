using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;

namespace BulkyBook.DataAccess.Repository;

public class UnitOfWork : IUnitOfWork{
    private ApplicationDbContext _db;
    public ICategoryRepository category{ get; private set; }
    public IProductRepository product{ get; private set; }
    public IComponyRepository compony{ get; private set; }
    public IShoppingCartRepository shoppingCart{ get; private set; }
    public IApplicationUserRepository applicationUser{ get; private set; }

    public UnitOfWork(ApplicationDbContext db){
        _db = db;
        category = new CategoryRepository(_db);
        product = new ProductRepository(_db);
        compony = new ComponyRepository(_db);
        shoppingCart = new ShoppingCartRepository(_db);
        applicationUser = new ApplicationUserRepository(_db);
    }

    public void Save(){
        _db.SaveChanges();
    }
}