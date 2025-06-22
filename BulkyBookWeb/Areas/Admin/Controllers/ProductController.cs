using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.View_Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

public class ProductController : Controller{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _iwebHostEnvironment;

    public ProductController(IUnitOfWork db, IWebHostEnvironment iwebHostEnvironment){
        _unitOfWork = db;
        _iwebHostEnvironment = iwebHostEnvironment;
    }

    public IActionResult Index(){
        var productsObj = _unitOfWork.product.GetAll().ToList();
        return View(productsObj);
    }

    public IActionResult Upsert(int? id){
        ProductVM productVm = new(){
            Product = new Product(),
            CategoryList = _unitOfWork.category.GetAll().Select(u => new SelectListItem{
                Text = u.Name,
                Value = u.Id.ToString()
            })
        };
        // if id is null, then it is a new product
        if (id == null || id == 0) return View(productVm);

        // if id is not null, then it is an existing product
        productVm.Product = _unitOfWork.product.Get(u => u.Id == id);
        return View(productVm);
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVm, IFormFile? file){
        if (ModelState.IsValid){
            string wwwRootPath = _iwebHostEnvironment.WebRootPath;

            if (file != null){
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images/products");

                if (!string.IsNullOrEmpty(productVm.Product.ImageUrl)){
                    var oldImagePath = Path.Combine(wwwRootPath, productVm.Product.ImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath)){
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create)){
                    file.CopyTo(fileStream);
                }

                productVm.Product.ImageUrl = @"/images/products/" + fileName;
            }

            if (productVm.Product.Id != 0){
                _unitOfWork.product.Update(productVm.Product);
            }
            else{
                _unitOfWork.product.Add(productVm.Product);
            }

            _unitOfWork.Save();
            TempData["success"] = $"Product '{productVm.Product.Title}' was created successfully";
            return RedirectToAction("Index");
        }
        else{
            productVm.CategoryList = _unitOfWork.category.GetAll().Select(u => new SelectListItem{
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(productVm);
        }
    }

    public IActionResult Delete(int? id){
        if (id == null || id == 0) return NotFound();
        var product = _unitOfWork.product.Get(u => u.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    [ActionName("Delete")]
    public IActionResult DeletePost(int? id){
        if (id == null || id == 0) return NotFound();
        var product = _unitOfWork.product.Get(u => u.Id == id);
        if (product == null) return NotFound();
        _unitOfWork.product.Delete(product);
        _unitOfWork.Save();
        TempData["success"] = $"Product '{product.Title}' was Deleted successfully";
        return RedirectToAction("Index");
    }
}