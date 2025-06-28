using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models.Models;

public class ShoppingCart{
    [Key] public int Id{ get; set; }

    public int ProductId{ get; set; }

    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product{ get; set; }

    [Range(1, 1000, ErrorMessage = "Count must be between 1 and 1000")]
    public int Count{ get; set; }

    public string? UserId{ get; set; }
    [ForeignKey("UserId")] [ValidateNever] public ApplicationUser User{ get; set; }

    [NotMapped] public double Price{ get; set; }
}