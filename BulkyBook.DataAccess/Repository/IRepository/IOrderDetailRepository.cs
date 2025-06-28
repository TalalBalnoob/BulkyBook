using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>{
    public void Update(OrderDetail orderDetail);
}