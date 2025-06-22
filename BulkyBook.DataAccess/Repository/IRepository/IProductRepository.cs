using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IProductRepository : IRepository<Product>{
    public void Update(Product product);
}