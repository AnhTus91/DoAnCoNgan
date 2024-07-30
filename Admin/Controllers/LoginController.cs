using PetShop.Areas.Admin.AdminModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;


namespace PetShop.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel model)
        {
            var result = Login(model);
            if(result == "Success")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["IncorrectAdmin"] = "Tên tài khoản hoặc mật khẩu không chính xác.";
                return this.Index();
            }
            
        }

        public string Login(LoginViewModel model)
        {
            var admin = db.Admins.Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefault();
            if (admin != null)
            {
                Session["AdminLogin"] = admin;
                return "Success";
            }
            else
            {
                return "IncorrectAdmin";
            }
        }
    }
}