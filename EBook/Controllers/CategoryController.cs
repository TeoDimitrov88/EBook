using EBook.Data;
using EBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace EBook.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> CategoryList= _context.Categories;
            return View(CategoryList);
        }
    }
}
