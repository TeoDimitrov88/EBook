using EBook.DataAccess.Data;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;

namespace EBook.DataAccess.Repository
{
    public class ProductRepository: Repository<Product>,IProductRepository
    {
        private ApplicationDbContext context;
        public ProductRepository(ApplicationDbContext _context): base(_context)
        {
            context = _context;       
        }

        public void Update(Product product)
        {
            var productFromDb = context.Products.FirstOrDefault(p=>p.Id==product.Id);
            if (productFromDb != null) 
            {
                productFromDb.Title = product.Title;
                productFromDb.Description = product.Description;
                productFromDb.Price = product.Price;
                productFromDb.ISBN = product.ISBN;
                productFromDb.priceFor25= product.priceFor25;
                productFromDb.priceFor50= product.priceFor50;
                productFromDb.ListPrice = product.ListPrice;
                productFromDb.CategoryId = product.CategoryId;
                productFromDb.CoverTypeId = product.CoverTypeId;    
                productFromDb.Author= product.Author;
                if (product.ImageURL != null)
                {
                    productFromDb.ImageURL = product.ImageURL;
                }
            }
        }
    }
}
