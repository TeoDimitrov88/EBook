using EBook.DataAccess;
using EBook.DataAccess.Repository;
using EBook.DataAccess.Repository.IRepository;
using EBook.Models;
using EBook.Models.Models;
using EBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public CompanyController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        //Get method

        public IActionResult Upsert(int? id)
        {
            Company company = new();


            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                company = unitOfWork.Company.GetFirstOrDefault(i => i.Id == id);
                return View(company);
            }
        }

        //Post method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company created successfully";

                }
                else
                {
                    unitOfWork.Company.Update(obj);
                    TempData["success"] = "Company updated successfully";

                }

                unitOfWork.Save();
               
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }


            unitOfWork.Company.Remove(obj);
            unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successfull!" });

        }
        #endregion
    }
}
