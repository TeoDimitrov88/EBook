using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;
using EBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EBookWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class ShoppingCartController : Controller
	{
		private readonly IUnitOfWork unitOfWork;

		public ShoppingCartVM ShoppingCartVM { get; set; }
		public ShoppingCartController(IUnitOfWork _unitOfWork)
		{
			unitOfWork = _unitOfWork;
		}
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				ListCart = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
				Order = new()
			};

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.priceFor25, cart.Product.priceFor50);
				ShoppingCartVM.Order.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}

		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				ListCart = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
				Order = new()
			};
			ShoppingCartVM.Order.ApplicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

			ShoppingCartVM.Order.Name = ShoppingCartVM.Order.ApplicationUser.Name;
			ShoppingCartVM.Order.PhoneNumber=ShoppingCartVM.Order.ApplicationUser.PhoneNumber;
			ShoppingCartVM.Order.StreetAddress = ShoppingCartVM.Order.ApplicationUser.StreetAddress;
			ShoppingCartVM.Order.City = ShoppingCartVM.Order.ApplicationUser.City;
			ShoppingCartVM.Order.State = ShoppingCartVM.Order.ApplicationUser.State;
			ShoppingCartVM.Order.PostCode = ShoppingCartVM.Order.ApplicationUser.PostCode;


			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.priceFor25, cart.Product.priceFor50);
				ShoppingCartVM.Order.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}

		public IActionResult Remove(int cartId)
		{
			var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			unitOfWork.ShoppingCart.Remove(cart);
			unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Plus(int cartId)
		{
			var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			unitOfWork.ShoppingCart.IncrementCount(cart, 1);
			unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId);
			if (cart.Count <= 1)
			{
				unitOfWork.ShoppingCart.Remove(cart);
			}
			else
			{
				unitOfWork.ShoppingCart.DecrementCount(cart, 1);
			}

			unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		private decimal GetPriceBasedOnQuantity(double quantity, decimal price, decimal priceFor25, decimal priceFor50)
		{
			if (quantity <= 25)
			{
				return price;
			}
			else
			{
				if (quantity <= 50)
				{
					return priceFor25;
				}
				return priceFor50;
			}
		}
	}
}
