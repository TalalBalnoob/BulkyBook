namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IUnitOfWork{
    ICategoryRepository category{ get; }
    IProductRepository product{ get; }
    IComponyRepository compony{ get; }
    IShoppingCartRepository shoppingCart{ get; }
    IApplicationUserRepository applicationUser{ get; }

    void Save();
}