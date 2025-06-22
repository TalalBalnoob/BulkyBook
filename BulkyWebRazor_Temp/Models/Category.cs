using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWebRazor_Temp.Models;

public class Category{
    [Key] public int Id{ get; set; }

    [DisplayName("Category Name")]
    [MaxLength(50, ErrorMessage = "Category Name must be less than 50 characters")]
    [Required]
    public string Name{ get; set; }

    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100")]
    public int DisplayOrder{ get; set; }
}