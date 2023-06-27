using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;
using EBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork _unitOfWork)
        {
            _logger = logger;
            unitOfWork = _unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart shoppingCart = new()
            {
                Count = 1,
                Product = unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType")
            };
            return View(shoppingCart);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
