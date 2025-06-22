using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyBook.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BulkyBook.Models.View_Models;

public class ProductVM{
    public Product Product{ get; set; }
    [ValidateNever] public IEnumerable<SelectListItem> CategoryList{ get; set; }
}