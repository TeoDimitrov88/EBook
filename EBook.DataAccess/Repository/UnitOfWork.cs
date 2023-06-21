using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext context;
        public UnitOfWork(ApplicationDbContext _context)
        {
            context = _context;
            Category = new CategoryRepository(context);
            CoverType= new CoverTypeRepository(context);
        }
        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get;private  set; }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}
