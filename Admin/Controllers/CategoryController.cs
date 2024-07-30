using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PetShop.Models;

namespace PetShop.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Category
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllCategories(int? page, string searchStr)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (String.IsNullOrEmpty(searchStr))
            {
                searchStr = " ";
            }

            var categories = db.Categories.Where(x => x.Name.Contains(searchStr) 
            || x.ParentCategory.Name.Contains(searchStr)
            || x.Pet.Name.Contains(searchStr)).ToList();

            ViewBag.ParentId = new SelectList(db.ParentCategories.ToList(), "Id", "Name");
            ViewBag.PetId = new SelectList(db.Pets.ToList(), "Id", "Name");

            return View(categories.ToPagedList(page ?? 1, 5));
        }

        public ActionResult AddNew()
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.ParentId = new SelectList(db.ParentCategories.ToList(), "Id", "Name");
            ViewBag.PetId = new SelectList(db.Pets.ToList(), "Id", "Name");
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddNew(Category category)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (ModelState.IsValid)
            {
                db.Categories.InsertOnSubmit(category);
                db.SubmitChanges();
                return RedirectToAction("ListAllCategories");
            }
            return this.AddNew();
        }

        [HttpPost]
        public JsonResult CategoryInfo(long id)
        {

            var category = db.Categories.Where(x => x.Id == id).FirstOrDefault();
            if(category != null)
            {
                return Json(new {
                    CategoryName = category.Name,
                    ParentId = category.ParentId,
                    PetId = category.PetId
                });
            }
            else
            {
                return Json("CannotFindCategory", JsonRequestBehavior.AllowGet);
            }
           
        }

        [HttpPost]
        public JsonResult UpdateCategory(long id, string name, int parentId, int petId)
        {
 
            var category = db.Categories.Where(x => x.Id == id).FirstOrDefault();

            if(category != null)
            {
                category.Name = name;
                category.ParentId = parentId;
                category.PetId = petId;
                UpdateModel(category);
                db.SubmitChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindCategory", JsonRequestBehavior.AllowGet);
            }
           
        }


        [HttpDelete]
        public ActionResult Delete(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var category = db.Categories.Where(x => x.Id == id).FirstOrDefault();
            if (category != null)
            {
                db.Categories.DeleteOnSubmit(category);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}