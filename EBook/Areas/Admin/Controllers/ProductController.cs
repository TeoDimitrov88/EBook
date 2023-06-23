using EBook.DataAccess;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;
using EBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ProductController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> coverTypeList = unitOfWork.CoverType.GetAll();
            return View(coverTypeList);
        }
        //Get method
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new()
            {
                Product = new(),
                CategoryList = unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            if (id == null || id == 0)
            {
                //create product
                return View(productViewModel);
            }
            else
            {
                //update product
            }
            return View(productViewModel);
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel product, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            //unitOfWork.CoverType.Add(product);
            unitOfWork.Save();
            TempData["success"] = "Cover Type updated successfully";

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
            //var coverTypeFromDb = context.CoverType.Find(id);
            var coverTypeFromDb = unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

            if (coverTypeFromDb == null)
            {
                return NotFound();
            }
            return View(coverTypeFromDb);
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType coverType)
        {
            if (!ModelState.IsValid)
            {
                return View(coverType);
            }
            unitOfWork.CoverType.Update(coverType);
            unitOfWork.Save();
            TempData["success"] = "Cover Type updated successfully";

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

            var coverTypeFromDb = unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (coverTypeFromDb == null)
            {
                return NotFound();
            }
            return View(coverTypeFromDb);
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int id)
        {
            var coverType = unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
            if (coverType == null)
            {
                return NotFound();
            }
            unitOfWork.CoverType.Remove(coverType);
            unitOfWork.Save();
            TempData["success"] = "Cover Type deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
