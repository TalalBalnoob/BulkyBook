using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>{
    public void Update(Category category);
}