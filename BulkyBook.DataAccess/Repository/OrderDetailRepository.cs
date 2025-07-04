using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;

namespace BulkyBook.DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository{
    private readonly ApplicationDbContext _db;

    public OrderDetailRepository(ApplicationDbContext db) : base(db){
        _db = db;
    }

    public void Update(OrderDetail orderDetail){
        _db.OrderDetails.Update(orderDetail);
    }

    
}