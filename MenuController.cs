using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;

namespace PetShop.Controllers
{
    public class MenuController : Controller
    {

        PetShopDataContext db = new PetShopDataContext();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuFood(int parentId = 1) // danh mục bánh ngọt 1 
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId).ToList();
            return PartialView(categories);
        }

        public ActionResult MenuAccessory(int parentId = 2)// danh mục bánh kem 2
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId).ToList();
            return PartialView(categories);
        }

        public ActionResult MenuToy(int parentId = 3)
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId).ToList();
            return PartialView(categories);
        }
    }
}