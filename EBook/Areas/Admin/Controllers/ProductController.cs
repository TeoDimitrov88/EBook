using EBook.DataAccess;
using EBook.DataAccess.Common;
using EBook.DataAccess.Repository;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;
using EBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IUnitOfWork _unitOfWork, IWebHostEnvironment _webHostEnvironment)
        {
            unitOfWork = _unitOfWork;
            webHostEnvironment = _webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
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
                productViewModel.Product = unitOfWork.Product.GetFirstOrDefault(i => i.Id == id);
                return View(productViewModel);
                //update product
            }
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel obj, IFormFile file)
        {
            if (ModelState.IsValid)
            {

                string wwwRootPath = webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Product.ImageURL != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageURL.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    };
                    obj.Product.ImageURL = @"\images\products\" + fileName + extension;
                }
                if (obj.Product.Id == 0)
                {
                    unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    unitOfWork.Product.Update(obj.Product);
                }

                unitOfWork.Save();
                TempData["success"] = "Product created successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(obj);
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

       
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }

        
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message= "Error while deleting"});
            }

            var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, obj.ImageURL.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            unitOfWork.Product.Remove(obj);
            unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull!" });

        }
        #endregion
    }
}
