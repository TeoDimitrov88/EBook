using EBook.DataAccess.Common;
using EBook.DataAccess.Repository;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;
using EBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace EBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
		
		[BindProperty]
		public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork _unitOfWork)
        {
			unitOfWork = _unitOfWork;  
        }
        public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderVM = new OrderVM()
			{
				Order=unitOfWork.Order.GetFirstOrDefault(u=>u.Id==orderId,includeProperties:"ApplicationUser"),
              OrderDetail   = unitOfWork.OrderDetails.GetAll(u => u.OrderId == orderId, includeProperties: "Product")

            };
			return View(OrderVM);
		}

		[ActionName("Details")]
		[HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult Details_PAY_NOW()
        {
			OrderVM.Order = unitOfWork.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id, includeProperties: "ApplicationUser");
			OrderVM.OrderDetail = unitOfWork.OrderDetails.GetAll(u => u.OrderId == OrderVM.Order.Id, includeProperties: "Product");

            //stripe settings 
            var domain = "https://localhost:44305/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                  "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderId={OrderVM.Order.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.Order.Id}",
            };

            foreach (var item in OrderVM.OrderDetail)
            {

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
                        Currency = "bgn",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);

            }

            var service = new SessionService();
            Session session = service.Create(options);
            unitOfWork.Order.UpdateStripePaymentId(OrderVM.Order.Id, session.Id, session.PaymentIntentId);
            unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderId)
        {
            Order order = unitOfWork.Order.GetFirstOrDefault(u => u.Id == orderId);
            if (order.PaymentStatus == Constants.PaymentDelayedStatus)
            {
                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                //check the stripe settings
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unitOfWork.Order.UpdateStripePaymentId(orderId, order.SessionId, session.PaymentIntentId);
                    unitOfWork.Order.UpdateStatus(orderId, order.OrderStatus, Constants.PaymentApprovedStatus);
                    unitOfWork.Save();
                }
            }
            return View(orderId);

        }

        [HttpPost]
		[Authorize(Roles =Constants.AdminRole + "," + Constants.EmployeeRole)]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetail()
		{
			var orderFromDb = unitOfWork.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id,tracked: false);
			orderFromDb.Name= OrderVM.Order.Name;
			orderFromDb.PhoneNumber=OrderVM.Order.PhoneNumber;
			orderFromDb.StreetAddress=OrderVM.Order.StreetAddress;
			orderFromDb.City=OrderVM.Order.City;
			orderFromDb.State=OrderVM.Order.State;
			orderFromDb.PostCode=OrderVM.Order.PostCode;
			if (OrderVM.Order.Carrier!= null)
			{
				orderFromDb.Carrier=OrderVM.Order.Carrier;
			}
			if (OrderVM.Order.TrackingNumber!=null)
			{
				orderFromDb.TrackingNumber=OrderVM.Order.TrackingNumber;
			}
			unitOfWork.Order.Update(orderFromDb);
			unitOfWork.Save();
			TempData["Success"] = "Order Details Updated Successfully.";

			return RedirectToAction("Details","Order", new { orderId = orderFromDb.Id});
		}

		[HttpPost]
		[Authorize(Roles = Constants.AdminRole + "," + Constants.EmployeeRole)]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcessing()
		{
			
			unitOfWork.Order.UpdateStatus(OrderVM.Order.Id,Constants.InProcessStatus);
			unitOfWork.Save();
			TempData["Success"] = "Order Status Updated Successfully.";

			return RedirectToAction("Details", "Order", new { orderId = OrderVM.Order.Id });
		}

		[HttpPost]
		[Authorize(Roles = Constants.AdminRole + "," + Constants.EmployeeRole)]
		[ValidateAntiForgeryToken]
		public IActionResult ShipOrder()
		{
			var order = unitOfWork.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id, tracked: false);
			order.TrackingNumber= OrderVM.Order.TrackingNumber;
			order.Carrier = OrderVM.Order.Carrier;
			order.OrderStatus = Constants.ShippedStatus;
			order.ShippingDate = DateTime.Now;
			if (order.PaymentStatus==Constants.PaymentDelayedStatus)
			{
				order.PaymentDueDate = DateTime.Now.AddDays(30);
			}
			unitOfWork.Order.Update(order);
			unitOfWork.Save();
			TempData["Success"] = "Order Shipped Successfully.";

			return RedirectToAction("Details", "Order", new { orderId = OrderVM.Order.Id });
		}

		[HttpPost]
		[Authorize(Roles = Constants.AdminRole + "," + Constants.EmployeeRole)]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var order = unitOfWork.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id, tracked: false);

			if (order.PaymentStatus==Constants.PaymentApprovedStatus)
			{
				var options = new RefundCreateOptions
				{
					Reason=RefundReasons.RequestedByCustomer,
					PaymentIntent= order.PaymentIntenId
				};

				var service = new RefundService();
				Refund refund = service.Create(options);

				unitOfWork.Order.UpdateStatus(order.Id, Constants.CancelledStatus, Constants.RefundedStatus);
			}
			else
			{
				unitOfWork.Order.UpdateStatus(order.Id, Constants.CancelledStatus, Constants.CancelledStatus);
			}

			unitOfWork.Save();
			TempData["Success"] = "Order Cancelled Successfully.";

			return RedirectToAction("Details", "Order", new { orderId = OrderVM.Order.Id });
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<Order> orders;

			if (User.IsInRole(Constants.AdminRole) || User.IsInRole(Constants.EmployeeRole))
			{
			orders = unitOfWork.Order.GetAll(includeProperties: "ApplicationUser");
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
				orders = unitOfWork.Order.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
			}

			switch (status)
			{
                case "pending":
					orders = orders.Where(s => s.PaymentStatus == Constants.PaymentPendingStatus);
                    break;
                case "inprocess":
                    orders = orders.Where(s => s.OrderStatus == Constants.InProcessStatus);
                    break;
                case "completed":
                    orders = orders.Where(s => s.OrderStatus == Constants.ShippedStatus);
                    break;
                case "approved":
                    orders = orders.Where(s => s.PaymentStatus == Constants.ApprovedStatus);
                    break;
                default:
                    break;
            }

			return Json(new { data = orders });
		}
		#endregion
	}
}
