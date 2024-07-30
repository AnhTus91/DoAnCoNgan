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
    public class ShoppingController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Shopping
        public ActionResult Index()
        {
            return View();
        }

        //Shop for dog

        public ActionResult ShopForDog(int? page)
        {
            var products = db.Products.Where(x => x.PetId == 1).ToList();
            return View(products.ToPagedList(page ?? 1, 12));
        }

        public ActionResult MenuFoodForDog(int parentId = 1, int petId = 1) // danh mục bánh ngọt 1 parentId 
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
            return PartialView(categories);
        }

        public ActionResult MenuAccessoryForDog(int parentId = 2, int petId = 1)// danh mục bánh kem 2
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
            return PartialView(categories);
        }

        //public ActionResult MenuToyForDog(int parentId = 3, int petId = 1)
        //{
        //    var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
        //    return PartialView(categories);
        //}

        //Shop for cat

         public ActionResult ShopForCat(int? page)
        {
            var products = db.Products.Where(x => x.PetId == 1).ToList();
            return View(products.ToPagedList(page ?? 1, 12));
        }

        public ActionResult MenuFoodForCat(int parentId = 1, int petId = 3) // danh mục bánh ngọt 1 parentId 
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
            return PartialView(categories);
        }

        public ActionResult MenuAccessoryForCat(int parentId = 2, int petId = 2) // danh mục bánh kem 2
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
            return PartialView(categories);
        }

        //public ActionResult MenuToyForCat(int parentId = 3, int petId = 2)
        //{
        //    var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
        //    return PartialView(categories);
        //}
    }
}