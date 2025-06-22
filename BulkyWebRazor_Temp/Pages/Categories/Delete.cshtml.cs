using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories;

public class Delete : PageModel{
    private readonly ApplicationDbContext _db;
    [BindProperty] public Category Category{ get; set; }

    public Delete(ApplicationDbContext db){
        _db = db;
    }

    public void OnGet(int? id){
        if (id == null || id == 0) return;
        Category = _db.Categories.Find(id);
        if (Category == null) return;
    }

    public IActionResult OnPost(){
        _db.Categories.Remove(Category);
        _db.SaveChanges();
        TempData["success"] = $"Category '{Category.Name}' was Deleted successfully";
        return RedirectToPage("./Index");
    }
}