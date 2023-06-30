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
    public class OrderDetailsRepository : Repository<OrderDetail>, IOrderDetailsRepository
    {
        private ApplicationDbContext context;
        public OrderDetailsRepository(ApplicationDbContext _context): base(_context)
        {
            context = _context;
        }

		public void Update(OrderDetail orderDetail)
		{
			throw new NotImplementedException();
		}
	}
}
