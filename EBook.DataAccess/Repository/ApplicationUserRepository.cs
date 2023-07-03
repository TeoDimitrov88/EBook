using EBook.DataAccess.Data;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;

namespace EBook.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>,IApplicationUserRepository
    {
        private ApplicationDbContext context;
        public ApplicationUserRepository(ApplicationDbContext _context): base(_context)
        {
            context = _context;       
        }

    }
}
