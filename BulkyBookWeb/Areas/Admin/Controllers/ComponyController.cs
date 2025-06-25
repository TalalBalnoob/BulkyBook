using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.RoleAdmin)]
public class ComponyController : Controller{
    private readonly IUnitOfWork _unitOfWork;

    public ComponyController(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(){
        var componiesList = _unitOfWork.compony.GetAll().ToList();
        return View(componiesList);
    }

    public IActionResult Upsert(int? id){
        if (id == null || id == 0) return View(new Compony());

        var compony = _unitOfWork.compony.Get(u => u.Id == id);
        if (compony == null) return NotFound();

        return View(compony);
    }

    [HttpPost]
    public IActionResult Upsert(Compony compony){
        if (ModelState.IsValid){
            if (compony.Id != 0){
                _unitOfWork.compony.Update(compony);
                TempData["success"] = $"Compony '{compony.Name}' was updated successfully";
            }
            else{
                _unitOfWork.compony.Add(compony);
                TempData["success"] = $"Compony '{compony.Name}' was created successfully";
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        return View(compony);
    }

    #region ApiCalls

    [HttpGet]
    public IActionResult GetAll(){
        var componiesList = _unitOfWork.compony.GetAll().ToList();
        return Json(componiesList);
    }

    public IActionResult Delete(int id){
        var componyToBeDeleted = _unitOfWork.compony.Get(u => u.Id == id);
        if (componyToBeDeleted == null){
            return Json(new{ success = false, message = "Compony not found" });
        }

        _unitOfWork.compony.Delete(componyToBeDeleted);
        _unitOfWork.Save();
        TempData["success"] = $"Compony '{componyToBeDeleted.Name}' was deleted successfully";
        return Json(new{ success = true, message = "Compony deleted successfully" });
    }

    #endregion
}