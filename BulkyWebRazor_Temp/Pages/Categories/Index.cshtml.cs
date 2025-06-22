using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories;

public class Index : PageModel{
    private readonly ApplicationDbContext _db;
    public List<Category> CategoryList{ get; set; }

    public Index(ApplicationDbContext db){
        _db = db;
    }

    public void OnGet(){
        CategoryList = _db.Categories.ToList();
    }
}