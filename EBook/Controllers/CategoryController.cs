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
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) 
            {
                return View(category);
            }
            context.Categories.Add(category);
            context.SaveChanges();
            TempData["success"] = "Category created successfully";
           
            return RedirectToAction(nameof(Index));
        }

        //Get method
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null|| id==0)
            {
                return NotFound();
            }
            var categoryFromDb = context.Categories.Find(id);
           
            if (categoryFromDb == null) 
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            context.Categories.Update(category);
            context.SaveChanges();
            TempData["success"] = "Category updated successfully";

            return RedirectToAction(nameof(Index));
        }

        //Get method
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var categoryFromDb = context.Categories.Find(id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int id)
        {
            var category= context.Categories.Find(id);
            if (category==null)
            {
                return NotFound();
            }
            context.Categories.Remove(category);
            context.SaveChanges();
            TempData["success"] = "Category deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
