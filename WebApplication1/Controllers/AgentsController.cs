using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AgentsController : Controller
    {
        private RealEstateEntities1 db = new RealEstateEntities1();

        // GET: Agents
        public ActionResult Index()
        {
            var agents = db.Agents.Include(a => a.User).Include(a => a.Payment);
            return View(agents.ToList());
        }

        // GET: Agents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // GET: Agents/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName");
            ViewBag.paymentId = new SelectList(db.Payments, "PaymentId", "PaymentName");
            return View();
        }

        // POST: Agents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AgentId,AgentName,Email,Address,Phone,isActivate,Password,Introduction,EmailHide,paymentId,UserId")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                db.Agents.Add(agent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", agent.UserId);
            ViewBag.paymentId = new SelectList(db.Payments, "PaymentId", "PaymentName", agent.paymentId);
            return View(agent);
        }
        
        // GET: Agents/Edit/5
        public ActionResult Edit(int? id)
        {
            if(id == null)
            {
                return HttpNotFound();
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", id);
            ViewBag.paymentId = new SelectList(db.Payments, "PaymentId", "PaymentName", id);
            return View(agent);
        }

        // POST: Agents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AgentName,AgentName,Email,Address,Phone,Introduction,EmailHide,isActivate,UserId")] Agent agent)
        {
            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {
                db.Entry(agent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName", agent.UserId);
            ViewBag.paymentId = new SelectList(db.Payments, "PaymentId", "PaymentName", agent.UserId);
            return View(agent);
        }

        // GET: Agents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // POST: Agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Agent agent = db.Agents.Find(id);
            db.Agents.Remove(agent);
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
            var id = Session["AgentId"];
            if (id != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Agents");
            }
        }

        public ActionResult Register()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "UserName");
            ViewBag.paymentId = new SelectList(db.Payments, "PaymentId", "PaymentName");
            return View();
        }

        //POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Agent agent)
        {
            if (ModelState.IsValid)
            {
                var check = db.Agents.FirstOrDefault(a => a.Email == agent.Email);
                if (check == null)
                {
                    agent.Password = GetMD5(agent.Password);
                    agent.ConfirmPassword = GetMD5(agent.ConfirmPassword);
                    agent.isActivate = false;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Agents.Add(agent);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Agents");
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
                var data = db.Agents.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).FirstOrDefault();
                if (data != null)
                {
                    if (data.isActivate == false)
                    {
                        ViewBag.Message = "Please wait for your account to be activated";
                        return View();
                    }
                    else
                    {
                        //add session
                        Session["AgentId"] = data.AgentId;
                        Session["AgentName"] = data.AgentName;
                        Session["Email"] = data.Email;
                        Session["Introduction"] = data.Introduction;
                        Session["Phone"] = data.Phone;
                        Session["Address"] = data.Address;
                        Session["EmailHide"] = Enum.GetName(typeof(EmailHide), data.EmailHide);
                        Session["isActivate"] = Enum.GetName(typeof(isActivate), data.isActivate);
                        Session["paymentId"] = data.paymentId;
                        return RedirectToAction("Information", "Agents");
                    }
                }
                else
                {
                    ViewBag.Message = "Wrong email or password";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        //create a string MD5
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
