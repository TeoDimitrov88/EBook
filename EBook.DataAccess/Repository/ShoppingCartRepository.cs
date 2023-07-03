using EBook.DataAccess.Data;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;

namespace EBook.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>,IShoppingCartRepository
    {
        private ApplicationDbContext context;
        public ShoppingCartRepository(ApplicationDbContext _context): base(_context)
        {
            context = _context;       
        }

        public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }
    }
}
