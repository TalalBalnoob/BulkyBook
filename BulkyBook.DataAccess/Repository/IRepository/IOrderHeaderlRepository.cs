using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IOrderHeaderRepository : IRepository<OrderHeader>{
    public void Update(OrderHeader orderHeader);
}