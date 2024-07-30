using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PetShop.Areas.Admin.AdminModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Bạn chưa nhập tài khoản.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu.")]
        public string  Password { get; set; }
    }
}