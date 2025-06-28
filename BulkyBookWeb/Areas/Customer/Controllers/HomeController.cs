using System.Diagnostics;
using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

[Area("Customer")]
public class HomeController : Controller{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork){
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(){
        var productsList = _unitOfWork.product.GetAll(propList: "Category");
        return View(productsList);
    }

    public IActionResult Details(int id){
        ShoppingCart shoppingCart = new(){
            Product = _unitOfWork.product.Get(u => id == u.Id, propList: "Category"),
            Count = 1,
            ProductId = id
        };

        return View(shoppingCart);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart){
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        shoppingCart.UserId = userId;

        // FIXME: this is my code check if it right
        var cartFormDb = _unitOfWork.shoppingCart.Get(u => u.UserId == userId && u.ProductId == shoppingCart.ProductId);
        if (cartFormDb == null){
            // work around to fix the issue of the id being set to the product id 
            shoppingCart.Id = 0;
            _unitOfWork.shoppingCart.Add(shoppingCart);
            _unitOfWork.Save();
        }
        else{
            cartFormDb.Count += shoppingCart.Count;
            _unitOfWork.shoppingCart.Update(cartFormDb);
            _unitOfWork.Save();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy(){
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(){
        return View(new ErrorViewModel{ RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}