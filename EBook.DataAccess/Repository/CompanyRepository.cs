using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;

namespace EBook.DataAccess.Repository
{
    public class CompanyRepository: Repository<Company>,ICompanyRepository
    {
        private ApplicationDbContext context;
        public CompanyRepository(ApplicationDbContext _context): base(_context)
        {
            context = _context;       
        }

        public void Update(Company company)
        {
            context.Companies.Update(company);
        }
    }
}
