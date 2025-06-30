using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.Models;
using BulkyBook.Models.View_Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

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