using EBook.DataAccess.Repository;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;
using Microsoft.AspNetCore.Mvc;

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
		public IActionResult GetAll()
		{
			IEnumerable<Order> orders;

			orders = unitOfWork.Order.GetAll(includeProperties: "ApplicationUser");
			return Json(new { data = orders });
		}
		#endregion
	}
}
