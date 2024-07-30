using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PetShop.Models;

namespace PetShop.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Order
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllOrders(int? page, string searchStr)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (String.IsNullOrEmpty(searchStr))
            {
                searchStr = string.Empty;
            }

            var orders = db.Orders.Where(x => x.Customer.FullName.Contains(searchStr)
                || x.Customer.Username.Contains(searchStr)).OrderByDescending(x => x.CreatedDate).ToList();

            ViewBag.StatusId = new SelectList(db.Status.ToList(), "Id", "Name");

            return View(orders.ToPagedList(page ?? 1, 5));
        }

        

        public ActionResult ListOrderDetailsByOrderId(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var orderDetails = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(orderDetails);
        }

        [HttpPost]
        public ActionResult OrderStatus(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
            return Json(order.StatusId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateStatus(long id, int statusId)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
            if(order != null)
            {
                order.StatusId = statusId;
                UpdateModel(order);
                db.SubmitChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindOrder", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult Delete(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var orderDetails = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            var order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
            if (orderDetails != null)
            {
                foreach(var item in orderDetails)
                {
                    db.OrderDetails.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }
            if (order != null)
            {
                db.Orders.DeleteOnSubmit(order);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult ExportReport()
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Lấy danh sách đơn hàng từ database (ví dụ sử dụng toàn bộ danh sách)
            var orders = db.Orders.ToList();

            // Tạo nội dung file CSV
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Mã khách hàng, Tên khách hàng, Số tiền");

            decimal totalAmount = 0;

            foreach (var order in orders)
            {
                // Sử dụng toán tử null-coalescing để đảm bảo giá trị luôn là số hợp lệ
                decimal orderTotal = order.TotalMoney ?? 0;
                // Sử dụng dấu ngoặc kép để bao quanh các trường có thể chứa dấu phẩy hoặc các ký tự đặc biệt
                string customerName = $"\"{order.Customer.FullName}\"";
                csvContent.AppendLine($"{order.Id}, {customerName}, {orderTotal}");
                totalAmount += orderTotal;
            }

            // Thêm dòng tổng tiền vào cuối file CSV
            csvContent.AppendLine();
            csvContent.AppendLine($"Tổng tiền,,, {totalAmount}");

            // Tên file xuất ra
            string fileName = "OrderReport.csv";

            // Chuẩn bị file để tải về
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment; filename={fileName}");
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF8;

            // Thêm BOM để đảm bảo mã hóa UTF-8
            byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF };
            Response.OutputStream.Write(bom, 0, bom.Length);

            // Ghi nội dung vào Response
            byte[] buffer = Encoding.UTF8.GetBytes(csvContent.ToString());
            Response.OutputStream.Write(buffer, 0, buffer.Length);
            Response.Flush();
            Response.End();

            return RedirectToAction("ListAllOrders"); // Chuyển hướng về danh sách đơn hàng sau khi xuất file
        }
        public ActionResult ExportProductQuantityReport()
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Lấy danh sách chi tiết đơn hàng từ database
            var orderDetails = db.OrderDetails.ToList();

            // Tạo một từ điển để lưu tổng số lượng của từng sản phẩm
            var productQuantities = new Dictionary<string, int>();

            foreach (var detail in orderDetails)
            {
                // Lấy thông tin sản phẩm
                var productName = detail.Product.Name;  // Nếu Product không null

                // Sử dụng toán tử null-coalescing để đảm bảo giá trị luôn là số hợp lệ
                int quantity = detail.Count ?? 0;

                // Kiểm tra giá trị của ProductName
                if (!string.IsNullOrEmpty(productName))
                {
                    // Nếu sản phẩm đã tồn tại trong từ điển, cộng dồn số lượng
                    if (productQuantities.ContainsKey(productName))
                    {
                        productQuantities[productName] += quantity;
                    }
                    else
                    {
                        // Nếu sản phẩm chưa tồn tại trong từ điển, thêm mới
                        productQuantities[productName] = quantity;
                    }
                }
            }

            // Tạo nội dung file CSV
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Tên sản phẩm, Số lượng");

            foreach (var item in productQuantities)
            {
                csvContent.AppendLine($"\"{item.Key}\", {item.Value}");
            }

            // Tên file xuất ra
            string fileName = "ProductQuantityReport.csv";

            // Chuẩn bị file để tải về
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", $"attachment; filename={fileName}");
            Response.ContentType = "text/csv";
            Response.ContentEncoding = Encoding.UTF8;

            // Thêm BOM để đảm bảo mã hóa UTF-8
            byte[] bom = new byte[] { 0xEF, 0xBB, 0xBF };
            Response.OutputStream.Write(bom, 0, bom.Length);

            // Ghi nội dung vào Response
            byte[] buffer = Encoding.UTF8.GetBytes(csvContent.ToString());
            Response.OutputStream.Write(buffer, 0, buffer.Length);
            Response.Flush();
            Response.End();

            return RedirectToAction("ListAllOrders"); // Chuyển hướng về danh sách đơn hàng sau khi xuất file
        }
    }
}