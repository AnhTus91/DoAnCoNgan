using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;
using PagedList;
using PagedList.Mvc;

namespace PetShop.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllProducts(int? page, string searchStr)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (String.IsNullOrEmpty(searchStr))
            {
                searchStr = " ";
            }

            var products = db.Products.Where(x => x.Name.ToLower().Contains(searchStr.ToLower())
            || x.Category.Name.ToLower().Contains(searchStr.ToLower())).ToList();

            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Suppliers.ToList(), "Id", "Name");

            return View(products.ToPagedList(page ?? 1, 5));
        }

       
        [HttpGet]
        public ActionResult AddNew()
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "Id", "Name");
            ViewBag.PetId = new SelectList(db.Pets.ToList(), "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Suppliers.ToList(), "Id", "Name");
            return PartialView();
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult AddNew(Product product)
        {

            product.Status = true;
            product.CreatedDate = DateTime.Now;
            if(product.Discount > 0)
            {
                product.PromotePrice = product.Price - (product.Price * product.Discount / 100);
            }
            db.Products.InsertOnSubmit(product);
            db.SubmitChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProductInfo(long id)
        {

            var product = db.Products.Where(x => x.Id == id).FirstOrDefault();
            if (product != null)
            {
                return Json(new
                {
                    Name = product.Name,
                    Price = product.Price,
                    Discount = product.Discount,
                    Image = product.Image,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    CategoryId = product.CategoryId,
                    SupplierId = product.SupplierId

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindProduct", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost, ValidateInput(false)]
        public JsonResult UpdateProduct(long id, string name, decimal price, string image, string description, int quantity, long categoryId, long supplierId)
        {

            var product = db.Products.Where(x => x.Id == id).FirstOrDefault();
            if (product != null)
            {
                product.Name = name;
                product.Price = price;
                product.Image = image;
                product.Description = description;
                product.CreatedDate = DateTime.Now;
                product.Quantity = quantity;
                product.CategoryId = categoryId;
                product.SupplierId = supplierId;
                UpdateModel(product);
                db.SubmitChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindProduct", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult Delete(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var product = db.Products.Where(x => x.Id == id).FirstOrDefault();
            if (product != null)
            {
                db.Products.DeleteOnSubmit(product);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}