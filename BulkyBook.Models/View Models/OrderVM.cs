using BulkyBook.Models.Models;

namespace BulkyBook.Models.View_Models;

public class OrderVM{
    public OrderHeader OrderHeader{ get; set; }
    public IEnumerable<OrderDetail> OrderDetails{ get; set; }
}