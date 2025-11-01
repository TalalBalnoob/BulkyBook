using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.View_Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BulkyBookWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class OrderController : Controller{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty] public OrderVM OrderVm{ get; set; }

    public OrderController(IUnitOfWork unitOfWork){
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index(){
        return View();
    }

    public IActionResult Details(int orderId){
        OrderVm = new(){
            OrderHeader = _unitOfWork.orderHeader.Get(u => u.Id == orderId, propList: "User"),
            OrderDetails = _unitOfWork.orderDetail.GetAll(filter: u => u.OrderHeaderId == orderId, propList: "Product")
        };

        return View(OrderVm);
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    public IActionResult UpdateOrderDetails(){
        var orderHeaderFromDb = _unitOfWork.orderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id);
        orderHeaderFromDb.Name = OrderVm.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = OrderVm.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = OrderVm.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = OrderVm.OrderHeader.City;
        orderHeaderFromDb.State = OrderVm.OrderHeader.State;
        orderHeaderFromDb.PostalCode = OrderVm.OrderHeader.PostalCode;
        if (!string.IsNullOrEmpty(OrderVm.OrderHeader.Carrier)){
            orderHeaderFromDb.Carrier = OrderVm.OrderHeader.Carrier;
        }

        if (!string.IsNullOrEmpty(OrderVm.OrderHeader.TrackingNumber)){
            orderHeaderFromDb.Carrier = OrderVm.OrderHeader.TrackingNumber;
        }

        _unitOfWork.orderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();

        TempData["Success"] = "Order Details Updated Successfully.";

        return RedirectToAction(nameof(Details), new{ orderId = OrderVm.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    public IActionResult StartProcessing(){
        _unitOfWork.orderHeader.UpdateStatus(OrderVm.OrderHeader.Id, SD.StatusInProcess);
        _unitOfWork.Save();
        TempData["Success"] = "Order Processing Started Successfully.";

        return RedirectToAction(nameof(Details), new{ orderId = OrderVm.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    public IActionResult ShipOrder(){
        var orderHeaderFromDb = _unitOfWork.orderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id);
        orderHeaderFromDb.TrackingNumber = OrderVm.OrderHeader.TrackingNumber;
        orderHeaderFromDb.Carrier = OrderVm.OrderHeader.Carrier;
        orderHeaderFromDb.OrderStatus = SD.StatusShipped;
        orderHeaderFromDb.ShippingDate = DateTime.Now;
        if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment){
            orderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        }

        _unitOfWork.orderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();
        TempData["Success"] = "Order Shipped Successfully.";

        return RedirectToAction(nameof(Details), new{ orderId = OrderVm.OrderHeader.Id });
    }

    [HttpPost]
    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    public IActionResult CancelOrder(){
        var orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id);

        if (orderHeader.PaymentStatus == SD.PaymentStatusApproved){
            var options = new RefundCreateOptions{
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            var refund = service.Create(options);

            _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
        }
        else{
            _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
        }

        _unitOfWork.Save();
        TempData["Success"] = "Order Cancelled Successfully.";
        return RedirectToAction(nameof(Details), new{ orderId = OrderVm.OrderHeader.Id });
    }

    [ActionName("Details")]
    [HttpPost]
    public IActionResult Details_PAY_NOW(){
        OrderVm.OrderHeader = _unitOfWork.orderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id, propList: "User");
        OrderVm.OrderDetails = _unitOfWork.orderDetail.GetAll(filter: u => u.OrderHeaderId == OrderVm.OrderHeader.Id,
            propList: "Product");

        var Domain = "http://localhost:5293/";
        var options = new SessionCreateOptions(){
            SuccessUrl = Domain + $"Admin/order/PaymentConfirmation?orderId={OrderVm.OrderHeader.Id}",
            CancelUrl = Domain + $"Admin/order/details?orderId={OrderVm.OrderHeader.Id}",
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
        };

        foreach (var item in OrderVm.OrderDetails){
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
        _unitOfWork.orderHeader.UpdateStripePaymentId(OrderVm.OrderHeader.Id, session.Id,
            session.PaymentIntentId);
        _unitOfWork.Save();
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult PaymentConfirmation(int orderId){
        OrderHeader orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == orderId);
        if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment){
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid"){
                _unitOfWork.orderHeader.UpdateStripePaymentId(orderId, session.Id, session.PaymentIntentId);
                _unitOfWork.orderHeader.UpdateStatus(orderId, orderHeader.OrderStatus,
                    SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
        }

        return View(orderId);
    }
    
    
    
    

    #region Api_Calls

    [HttpGet]
    public IActionResult GetAll(string status){
        IEnumerable<OrderHeader> ordersList;
        if (User.IsInRole(SD.RoleAdmin) || User.IsInRole(SD.RoleEmployee)){
            ordersList = _unitOfWork.orderHeader.GetAll("User").ToList();
        }
        else{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ordersList = _unitOfWork.orderHeader.GetAll("User", u => u.UserId == userId).ToList();
        }

        switch (status){
            case "pending":
                ordersList = ordersList.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                break;
            case "inprocess":
                ordersList = ordersList.Where(u => u.PaymentStatus == SD.StatusInProcess);
                break;
            case "complete":
                ordersList = ordersList.Where(u => u.PaymentStatus == SD.StatusShipped);
                break;
            case "approved":
                ordersList = ordersList.Where(u => u.PaymentStatus == SD.StatusApproved);
                break;
            default:
                break;
        }

        return Json(ordersList);
    }

    #endregion
}