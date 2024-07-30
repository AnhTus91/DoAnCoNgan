using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;
using PagedList;
using PagedList.Mvc;

namespace PetShop.Controllers
{
    public class HomeController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        public ActionResult Index()
        {
            var products = db.Products.Where(x => x.Discount == 1).Take(12).ToList();
            ViewBag.PromotionalProducts = db.Products.Where(x => x.Discount > 0).Take(6).ToList();
            return View(products);
        }

        public ActionResult Search(string searchStr)
        {
            var products = db.Products.Where(x => x.Name.ToLower().Contains(searchStr.ToLower()) || x.Supplier.Name.ToLower().Contains(searchStr.ToLower())).ToList();
            return View(products);
        }


        // product details
        public ActionResult Details(long id, int? page)
        {
            var product = db.Products.Single(x => x.Id == id);
            ViewBag.ProductsOfTheSameType = ListByCategoryId(product.CategoryId, 12);
            return View(product);
        }

        public List<Product> ListByCategoryId(long? id, int count)
        {
            var products = db.Products.Where(x => x.CategoryId == id).Take(count).ToList();
            return products;
        }

        // changed into the top 3 menu
        public ActionResult ListByMenuSide(long id)
        {
            var products = ListByCategoryId(id, 0);
            var category = db.Categories.Single(x => x.Id == id);
            ViewBag.CategoryTitle = category.Name;
            return View(products);
        }
        public JsonResult GetVisitStatistics()
        {
            var data = db.Products
                         .GroupBy(p => p.Id)
                         .Select(g => new
                         {
                             ProductId = g.Key,
                             Visits = g.Sum(p => p.VisitCount) // Assuming VisitCount field exists
                         }).ToList();
            System.Diagnostics.Debug.WriteLine("Visit Data: " + Newtonsoft.Json.JsonConvert.SerializeObject(data));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRevenueStatistics()
        {
            var data = db.Orders
                         .GroupBy(o => o.ProductId)
                         .Select(g => new
                         {
                             ProductId = g.Key,
                             Revenue = g.Sum(o => o.TotalMoney)
                         }).ToList();
            System.Diagnostics.Debug.WriteLine("Revenue Data: " + Newtonsoft.Json.JsonConvert.SerializeObject(data));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}