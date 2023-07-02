using EBook.DataAccess.Common;
using EBook.DataAccess.Repository;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
        public OrderController(IUnitOfWork _unitOfWork)
        {
			unitOfWork = _unitOfWork;  
        }
        public IActionResult Index()
		{
			return View();
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<Order> orders;

			orders = unitOfWork.Order.GetAll(includeProperties: "ApplicationUser");

			switch (status)
			{
                case "pending":
					orders = orders.Where(s => s.PaymentStatus == StaticConst.PaymentPendingStatus);
                    break;
                case "inprocess":
                    orders = orders.Where(s => s.OrderStatus == StaticConst.InProcessStatus);
                    break;
                case "completed":
                    orders = orders.Where(s => s.OrderStatus == StaticConst.ShippedStatus);
                    break;
                case "approved":
                    orders = orders.Where(s => s.PaymentStatus == StaticConst.ApprovedStatus);
                    break;
                default:
                    break;
            }

			return Json(new { data = orders });
		}
		#endregion
	}
}
