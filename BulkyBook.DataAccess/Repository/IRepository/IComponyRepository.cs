using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IComponyRepository : IRepository<Compony>{
    public void Update(Compony compony);
}