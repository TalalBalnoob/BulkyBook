using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models.Models;

public class Product{
    [Key] public int Id{ get; set; }
    [Required] public string Title{ get; set; }
    public string Description{ get; set; }
    [Required] public string ISBN{ get; set; }
    [Required] public string Author{ get; set; }

    [Display(Name = "List Price")]
    [Range(1, 1000)]
    [Required]
    public double ListPrice{ get; set; }

    [Display(Name = "Price for 1-50")]
    [Range(1, 1000)]
    [Required]
    public double Price{ get; set; }

    [Display(Name = "Price for 50+")]
    [Range(1, 1000)]
    [Required]
    public double Price50{ get; set; }

    [Display(Name = "Price for 100+")]
    [Range(1, 1000)]
    [Required]
    public double Price100{ get; set; }

    [ValidateNever] public int CategoryId{ get; set; }

    [ForeignKey("CategoryId")]
    [ValidateNever]
    public Category? Category{ get; set; }
    
    [ValidateNever] public string? ImageUrl{ get; set; }
}