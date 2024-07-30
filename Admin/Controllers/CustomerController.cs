using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;

namespace PetShop.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllCustomers()
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var customers = db.Customers.ToList();
            return View(customers);
        }

        public ActionResult Search(string searchStr)
        {
            var customers = db.Customers.Where(x => x.FullName.ToLower().Contains(searchStr.ToLower()) 
            || x.Username.ToLower().Contains(searchStr.ToLower()) 
            || x.Address.ToLower().Contains(searchStr.ToLower()) 
            || x.Username.ToLower().Contains(searchStr.ToLower())
            || x.Phone.ToLower().Contains(searchStr.ToLower())
            || x.Email.ToLower().Contains(searchStr.ToLower())).ToList();
            return View("ListAllCustomers", customers);
        }

        [HttpDelete]
        public ActionResult Delete(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var customer = db.Customers.Where(user => user.Id == id).FirstOrDefault();
            if (customer != null)
            {
                db.Customers.DeleteOnSubmit(customer);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public JsonResult Details(long id)
        {

            var customer = db.Customers.Where(x => x.Id == id).FirstOrDefault();
            if (customer != null)
            {
                return Json(new
                {
                    FullName = customer.FullName,
                    Gender = customer.Gender,
                    Username = customer.Username,
                    Address = customer.Address,
                    Phone = customer.Phone,
                    Email = customer.Email
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindCustomer", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update(long id, string fullName, bool gender, string username, string address, string phone, string email)
        {
            var customer = db.Customers.Where(x => x.Id == id).FirstOrDefault();
            if(customer == null)
            {
                return Json("Fail", JsonRequestBehavior.AllowGet);
            }
            customer.FullName = fullName;
            customer.Gender = gender;
            customer.Username = username;
            customer.Address = address;
            customer.Phone = phone;
            customer.Email = email;

            UpdateModel(customer);
            db.SubmitChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

       
    }
}