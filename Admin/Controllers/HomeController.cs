using PetShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShop.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        private PetShopDataContext db = new PetShopDataContext();
        public ActionResult Index()
        {
            if(Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        

    }
}