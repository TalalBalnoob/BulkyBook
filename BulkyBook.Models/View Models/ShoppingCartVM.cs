using BulkyBook.Models.Models;

namespace BulkyBook.Models.View_Models;

public class ShoppingCartVM{
    public IEnumerable<ShoppingCart> ShoppingCarts{ get; set; }
    public OrderHeader OrderHeader{ get; set; }
}