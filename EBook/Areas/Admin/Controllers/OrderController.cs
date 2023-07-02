using EBook.DataAccess.Common;
using EBook.DataAccess.Repository;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;
using EBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
