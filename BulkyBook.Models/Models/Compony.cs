using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models.Models;

public class Compony{
    [Key] public int Id{ get; set; }

    [Display(Name = "Company Name")]
    [Required]
    public string Name{ get; set; }

    [Display(Name = "Street Address")] public string? StreetAddress{ get; set; }
    public string? City{ get; set; }
    public string? State{ get; set; }
    [Display(Name = "Postal Code")] public string? PostalCode{ get; set; }
    [Display(Name = "Phone Number")] public string? PhoneNumber{ get; set; }
}