using EBook.DataAccess.Data;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;

namespace EBook.DataAccess.Repository
{
    public class CoverTypeRepository: Repository<CoverType>,ICoverTypeRepository
    {
        private ApplicationDbContext context;
        public CoverTypeRepository(ApplicationDbContext _context): base(_context)
        {
            context = _context;       
        }

        public void Update(CoverType coverType)
        {
            context.CoverTypes.Update(coverType);
        }
    }
}
