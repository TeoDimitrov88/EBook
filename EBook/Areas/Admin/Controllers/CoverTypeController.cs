using EBook.DataAccess;
using EBook.DataAccess.Common;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.AdminRole)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public CoverTypeController(IUnitOfWork _unitOfWork)
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
        public IActionResult Create()
        {
            return View();
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType coverType)
        {
            if (!ModelState.IsValid)
            {
                return View(coverType);
            }
            unitOfWork.CoverType.Add(coverType);
            unitOfWork.Save();
            TempData["success"] = "Cover Type created successfully";

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
