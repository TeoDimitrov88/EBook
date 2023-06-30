using EBook.DataAccess.Common;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;
using EBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace EBookWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class ShoppingCartController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
		
		[BindProperty]
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

		[HttpGet]
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

		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM.ListCart = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product");
				
			ShoppingCartVM.Order.PaymentStatus = StaticConst.PaymentPendingStatus;
			ShoppingCartVM.Order.OrderStatus = StaticConst.PendingStatus;
			ShoppingCartVM.Order.OrderDate = System.DateTime.Now;
			ShoppingCartVM.Order.ApplicationUserId = claim.Value;

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.priceFor25, cart.Product.priceFor50);
				ShoppingCartVM.Order.OrderTotal += (cart.Price * cart.Count);
			}

			unitOfWork.Order.Add(ShoppingCartVM.Order);
			unitOfWork.Save();
			foreach (var cart in ShoppingCartVM.ListCart)
			{
				OrderDetail orderDetails = new()
				{
					ProductId = cart.ProductId,
					OrderId = ShoppingCartVM.Order.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				unitOfWork.OrderDetails.Add(orderDetails);
				unitOfWork.Save();
			}

			//stripe settings
			//var domain= ""
			//var options= new SessionCreateOptions 
			//{
			//  PaymentMethodTypes= new List<string>()
			//  {
			//	  "card",
			//  },
			//  LineItems=new List<SessionLineItemOptions>(),
			//  Mode="payment",
			//  SuccessUrl= 
			//}


			unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
			unitOfWork.Save();

			return RedirectToAction(nameof(Index));
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
