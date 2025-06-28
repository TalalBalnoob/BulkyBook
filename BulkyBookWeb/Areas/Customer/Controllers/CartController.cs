using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.View_Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

[Area("Customer")]
public class CartController : Controller{
    private readonly IUnitOfWork _unitOfWork;

    public CartController(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(){
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var cart = new ShoppingCartVM(){
            ShoppingCarts = _unitOfWork.shoppingCart.GetAll(propList: "Product").Where(i => i.UserId == userId),
        };

        foreach (var item in cart.ShoppingCarts){
            item.Price = GetPriceBasedOnCount(item);
            cart.TotalPrice += (item.Count * item.Price);
        }

        return View(cart);
    }

    public IActionResult Summary(){
        return View();
    }

    public IActionResult Plus(int cartId){
        var shoppingCart = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
        if (shoppingCart == null) return NotFound();

        shoppingCart.Count++;
        _unitOfWork.shoppingCart.Update(shoppingCart);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int cartId){
        var shoppingCart = _unitOfWork.shoppingCart.Get(u => u.Id == cartId);
        if (shoppingCart == null) return NotFound();

        shoppingCart.Count--;
        if (!(shoppingCart.Count <= 0)){
            _unitOfWork.shoppingCart.Update(shoppingCart);
            _unitOfWork.Save();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Delete(int cartId){
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var shoppingCart = _unitOfWork.shoppingCart.Get(u => u.Id == cartId && u.UserId == userId);
        if (shoppingCart == null) return NotFound();

        _unitOfWork.shoppingCart.Delete(shoppingCart);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    private double GetPriceBasedOnCount(ShoppingCart shoppingCart){
        if (shoppingCart.Count <= 50) return shoppingCart.Product.Price;
        if (shoppingCart.Count <= 100) return shoppingCart.Product.Price50;
        else return shoppingCart.Product.Price100;
    }
}