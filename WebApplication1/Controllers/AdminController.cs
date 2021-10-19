using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private RealEstateEntities1 db = new RealEstateEntities1();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["UserId"] != null)
            {
                return View();
            }
            else
            {
                ViewBag.error = "Login failed";
                return RedirectToAction("Login", "Admin");
            }
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = db.Users.Where(s => s.UserName.Equals(username) && s.Password.Equals(f_password)).ToList();

                if (data.Count() > 0)
                {
                    Session["UserId"] = data.FirstOrDefault().UserId;
                    Session["UserName"] = data.FirstOrDefault().UserName;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.Message = "Wrong username or password";
                }
                return View();
            }
            return View();
        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}