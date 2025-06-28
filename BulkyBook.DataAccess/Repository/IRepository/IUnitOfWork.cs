namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IUnitOfWork{
    ICategoryRepository category{ get; }
    IProductRepository product{ get; }
    IComponyRepository compony{ get; }
    IShoppingCartRepository shoppingCart{ get; }
    IApplicationUserRepository applicationUser{ get; }
    IOrderHeaderRepository orderHeader{ get; }
    IOrderDetailRepository orderDetail{ get; }

    void Save();
}