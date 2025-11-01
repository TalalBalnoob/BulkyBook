using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent{
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartViewComponent(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync(){
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var calim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if (calim != null){
            if (HttpContext.Session.GetInt32(SD.SessionCart) == null){
                var userId = calim.Value;
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.shoppingCart.GetAll(filter: u => u.UserId == userId).Count());
            }

            return View(HttpContext.Session.GetInt32(SD.SessionCart) ?? 0);
        }
        else{
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}