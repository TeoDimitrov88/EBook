using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.DataAccess.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private ApplicationDbContext context;
        public OrderRepository(ApplicationDbContext _context): base(_context)
        {
            context = _context;
        }

		public void Update(Order order)
		{
			throw new NotImplementedException();
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = context.Orders.FirstOrDefault(u => u.Id == id);
			if (orderFromDb != null)
			{
			 orderFromDb.OrderStatus = orderStatus;
				if (paymentStatus!=null)
				{
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		}
	}
}
