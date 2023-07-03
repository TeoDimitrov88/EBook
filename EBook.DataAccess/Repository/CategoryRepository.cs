using EBook.DataAccess.Data;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext context;
        public CategoryRepository(ApplicationDbContext _context): base(_context)
        {
            context = _context;
        }
        public void Update(Category category)
        {
            context.Update(category);
        }
        
    }
}
