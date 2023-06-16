using EBook.Data;
using EBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace EBook.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext context;
        public CategoryController(ApplicationDbContext _context)
        {
            context = _context;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> CategoryList= context.Categories;
            return View(CategoryList);
        }
        //Get method
        public IActionResult Create()
        {
            return View();
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            context.Categories.Add(category);
            context.SaveChanges();
           
            return RedirectToAction(nameof(Index));
        }
    }
}
