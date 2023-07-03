using EBook.DataAccess;
using EBook.DataAccess.Common;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace EBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.AdminRole)]
    public class CategoryController : Controller
    {
        
        private readonly IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> CategoryList = unitOfWork.Category.GetAll();
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
            unitOfWork.Category.Add(category);
            unitOfWork.Save();
            TempData["success"] = "Category created successfully";

            return RedirectToAction(nameof(Index));
        }

        //Get method
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = context.Categories.Find(id);
            var categoryFromDb = unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

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
            unitOfWork.Category.Update(category);
            unitOfWork.Save();
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
            //var categoryFromDb = context.Categories.Find(id);
            var categoryFromDb = unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
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
            var category = unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            unitOfWork.Category.Remove(category);
            unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
