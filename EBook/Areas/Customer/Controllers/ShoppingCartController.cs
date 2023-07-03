using EBook.DataAccess.Common;
using EBook.DataAccess.Repository;
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
			ShoppingCartVM.Order.PhoneNumber = ShoppingCartVM.Order.ApplicationUser.PhoneNumber;
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

			ShoppingCartVM.Order.OrderDate = System.DateTime.Now;
			ShoppingCartVM.Order.ApplicationUserId = claim.Value;

			foreach (var cart in ShoppingCartVM.ListCart)
			{
				cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.priceFor25, cart.Product.priceFor50);
				ShoppingCartVM.Order.OrderTotal += (cart.Price * cart.Count);
			}

			ApplicationUser applicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				ShoppingCartVM.Order.PaymentStatus = Constants.PaymentPendingStatus;
				ShoppingCartVM.Order.OrderStatus = Constants.PendingStatus;

			}
			else
			{
				ShoppingCartVM.Order.PaymentStatus = Constants.PaymentDelayedStatus;
				ShoppingCartVM.Order.OrderStatus = Constants.ApprovedStatus;

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
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
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
					SuccessUrl = domain + $"customer/shoppingCart/orderConfirmation?id={ShoppingCartVM.Order.Id}",
					CancelUrl = domain + $"customer/shoppingCart/index",
				};

				foreach (var item in ShoppingCartVM.ListCart)
				{

					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
							Currency = "eur",
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
				unitOfWork.Order.UpdateStripePaymentId(ShoppingCartVM.Order.Id, session.Id, session.PaymentIntentId);
				unitOfWork.Save();
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}

			else
			{
				return RedirectToAction("OrderConfirmation", "ShoppingCart", new { id = ShoppingCartVM.Order.Id });
			}

			//unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
			//unitOfWork.Save();

			//return RedirectToAction(nameof(Index));
		}

		public IActionResult OrderConfirmation(int id)
		{
			Order order = unitOfWork.Order.GetFirstOrDefault(u => u.Id == id);
			if (order.PaymentStatus != Constants.PaymentDelayedStatus)
			{
				var service = new SessionService();
				Session session = service.Get(order.SessionId);

				//check the stripe settings
				if (session.PaymentStatus.ToLower() == "paid")
				{
					unitOfWork.Order.UpdateStripePaymentId(id, order.SessionId, session.PaymentIntentId);
					unitOfWork.Order.UpdateStatus(id, Constants.ApprovedStatus, Constants.PaymentApprovedStatus);
					unitOfWork.Save();
				}
			}

			List<ShoppingCart> shoppingCarts = unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == order.ApplicationUserId).ToList();
			unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			unitOfWork.Save();

			return View(id);

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
