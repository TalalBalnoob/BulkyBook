using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models.Models;

public class OrderDetail{
    public int Id{ get; set; }

    [Required] public int OrderHeaderId{ get; set; }

    [ForeignKey("OrderHeaderId")]
    [ValidateNever]
    public OrderHeader OrderHeader{ get; set; }

    [Required] public int ProductId{ get; set; }

    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product{ get; set; }

    [Required] public int Count{ get; set; }
    [Required] public double Price{ get; set; }
}