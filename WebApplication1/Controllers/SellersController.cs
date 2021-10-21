using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SellersController : Controller
    {
        private RealEstateEntities1 db = new RealEstateEntities1();

        // GET: Sellers
        public ActionResult Index()
        {
            var sellers = db.Sellers.Include(s => s.User);
            return View(sellers.ToList());
        }

        // GET: Sellers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // GET: Sellers/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName");
            return View();
        }

        // POST: Sellers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SellerId,Name,Gender,Email,Birthdate,Address,Phone,isActivate,Password,UserId")] Seller seller)
        {
            if (ModelState.IsValid)
            {
                db.Sellers.Add(seller);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", seller.UserId);
            return View(seller);
        }

        // GET: Sellers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", seller.UserId);
            return View(seller);
        }

        // POST: Sellers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SellerId,Name,Gender,Email,Birthdate,Address,Phone,isActivate,UserId")] Seller seller)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {
                db.Entry(seller).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", seller.UserId);
            return View(seller);
        }

        // GET: Sellers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seller seller = db.Sellers.Find(id);
            if (seller == null)
            {
                return HttpNotFound();
            }
            return View(seller);
        }

        // POST: Sellers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Seller seller = db.Sellers.Find(id);
            db.Sellers.Remove(seller);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Information()
        {
            if (Session["SellerId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Sellers");
            }
        }
        public ActionResult Register()
        {
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Seller seller)
        {
            if (ModelState.IsValid)
            {
                var check = db.Sellers.FirstOrDefault(s => s.Email == seller.Email);
                if (check == null)
                {
                    seller.Password = GetMD5(seller.Password);
                    seller.ConfirmPassword = GetMD5(seller.ConfirmPassword);
                    seller.isActivate = false;
                    db.Sellers.Add(seller);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Sellers");
                }
                else
                {
                    ViewBag.error = "Email already existed";
                    return View();
                }
            }
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = db.Sellers.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).FirstOrDefault();
                if (data != null)
                {
                    if (data.isActivate == false)
                    {
                        ViewBag.Message = "Please wait for your account to be activated";
                        return View();
                    }
                    else
                    {
                        Session["SellerId"] = data.SellerId;
                        Session["Name"] = data.Name;
                        Session["Email"] = data.Email;
                        Session["Birthdate"] = data.Birthdate.ToString();
                        Session["Phone"] = data.Phone;
                        Session["Address"] = data.Address;
                        Session["Gender"] = Enum.GetName(typeof(Gender), data.Gender);
                        Session["isActivate"] = Enum.GetName(typeof(isActivate), data.isActivate);
                        return RedirectToAction("Information", "Sellers");
                    }
                }
                else
                {
                    ViewBag.Message = "Wrong email or password";
                }
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
    }
}
