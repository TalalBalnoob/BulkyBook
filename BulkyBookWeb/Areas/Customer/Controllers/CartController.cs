using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.View_Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BulkyWeb.Controllers;

[Area("Customer")]
public class CartController : Controller{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty] public ShoppingCartVM ShoppingCartVM{ get; set; }

    public CartController(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(){
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        ShoppingCartVM = new(){
            ShoppingCarts =
                _unitOfWork.shoppingCart.GetAll(propList: "Product").Where(i => i.UserId == userId),
            OrderHeader = new()
        };

        foreach (var item in ShoppingCartVM.ShoppingCarts){
            item.Price = GetPriceBasedOnCount(item);
            ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);
        }

        return View(ShoppingCartVM);
    }

    public IActionResult Summary(){
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        ShoppingCartVM = new(){
            ShoppingCarts = _unitOfWork.shoppingCart.GetAll(propList: "Product").Where(i => i.UserId == userId),
            OrderHeader = new()
        };

        ShoppingCartVM.OrderHeader.User = _unitOfWork.applicationUser.Get(u => u.Id == userId);

        ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.User.Name;
        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.User.PhoneNumber;
        ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.User.State;
        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.User.PostalCode;
        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.User.City;
        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.User.StreetAddress;

        foreach (var item in ShoppingCartVM.ShoppingCarts){
            item.Price = GetPriceBasedOnCount(item);
            ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);
        }

        return View(ShoppingCartVM);
    }

    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPost(){
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        ShoppingCartVM.ShoppingCarts =
            _unitOfWork.shoppingCart.GetAll(propList: "Product").Where(i => i.UserId == userId);

        ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        ShoppingCartVM.OrderHeader.UserId = userId;

        ApplicationUser applicationUser = _unitOfWork.applicationUser.Get(u => u.Id == userId);

        foreach (var item in ShoppingCartVM.ShoppingCarts){
            item.Price = GetPriceBasedOnCount(item);
            ShoppingCartVM.OrderHeader.OrderTotal += (item.Count * item.Price);
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0){
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        }
        else{
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
        }

        _unitOfWork.orderHeader.Add(ShoppingCartVM.OrderHeader);
        _unitOfWork.Save();

        foreach (var cart in ShoppingCartVM.ShoppingCarts){
            OrderDetail orderDetail = new(){
                ProductId = cart.ProductId,
                Count = cart.Count,
                OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                Price = cart.Price
            };
            _unitOfWork.orderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0){
            var Domain = "http://localhost:5293/";
            var options = new SessionCreateOptions(){
                SuccessUrl = Domain + $"Customer/cart/OrderConfirmation/{ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = Domain + "Customer/Cart/Index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in ShoppingCartVM.ShoppingCarts){
                var sessionLineItemOptions = new SessionLineItemOptions{
                    PriceData = new SessionLineItemPriceDataOptions{
                        UnitAmount = (long)(100 * item.Price),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions{
                            Name = item.Product.Title,
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItemOptions);
            }


            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.orderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id,
                session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        return RedirectToAction(nameof(OrderConfirmation), new{ id = ShoppingCartVM.OrderHeader.Id });
    }

    public IActionResult OrderConfirmation(int id){
        OrderHeader orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == id, propList: "User");
        if (orderHeader.PaymentStatus != SD.PaymentStatusPending){
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid"){
                _unitOfWork.orderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
        }

        List<ShoppingCart> shoppingCarts =
            _unitOfWork.shoppingCart.GetAll(filter: u => u.UserId == orderHeader.UserId).ToList();
        _unitOfWork.shoppingCart.DeleteRange(shoppingCarts);
        _unitOfWork.Save();
        
        return View(id);
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