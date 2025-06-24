using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.RoleAdmin)]

public class CategoryController : Controller{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork db){
        _unitOfWork = db;
    }

    public IActionResult Index(){
        var categoriesObj = _unitOfWork.category.GetAll().ToList();
        return View(categoriesObj);
    }

    public IActionResult Create(){
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category){
        // Custom validation
        if (category.Name == category.DisplayOrder.ToString())
            // add Custom error to the model state with the error message
            ModelState.AddModelError("name", "Name cannot be the same as Display Order");

        if (ModelState.IsValid){
            _unitOfWork.category.Add(category);
            _unitOfWork.Save();
            TempData["success"] = $"Category '{category.Name}' was created successfully";
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Edit(int? id){
        if (id == null || id == 0) return NotFound();
        var category = _unitOfWork.category.Get(u => u.Id == id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category category){
        if (ModelState.IsValid){
            _unitOfWork.category.Update(category);
            _unitOfWork.Save();
            TempData["success"] = $"Category '{category.Name}' was Updated successfully";
            return RedirectToAction("Index");
        }

        return View();
    }

    public IActionResult Delete(int? id){
        if (id == null || id == 0) return NotFound();
        var category = _unitOfWork.category.Get(u => u.Id == id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [ActionName("Delete")]
    public IActionResult DeletePost(int? id){
        if (id == null || id == 0) return NotFound();
        var category = _unitOfWork.category.Get(u => u.Id == id);
        if (category == null) return NotFound();
        _unitOfWork.category.Delete(category);
        _unitOfWork.Save();
        TempData["success"] = $"Category '{category.Name}' was Deleted successfully";
        return RedirectToAction("Index");
    }
}