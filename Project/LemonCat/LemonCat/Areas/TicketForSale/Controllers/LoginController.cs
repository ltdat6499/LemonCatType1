using LemonCat.Areas.Admin.Models;
using LemonCat.Common;
using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LemonCat.Areas.TicketForSale.Controllers
{
    public class LoginController : Controller
    {
        // GET: TicketForSale/Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                bool result = UserDAO.Instance.StaffLogin(model.UserName, Encryptor.MD5Hash(model.Password));
                if (result)
                {
                    var user = UserDAO.Instance.GetByUserName(model.UserName);
                    var usersession = new UserLogin();
                    usersession.UserID = user.MaTK;
                    usersession.UserName = user.TenTK;
                    usersession.AvataPath = user.AnhDaiDien;
                    usersession.Name = user.LastName;
                    Session.Add(CommonConstaints.TICKETSTAFF_USER_SESSION, usersession);
                    return RedirectToAction("Index", "Home");
                }
                else if (UserDAO.Instance.IsLocked(model.UserName) != "")
                {
                    return RedirectToAction("Index", new RouteValueDictionary(
    new { controller = "Lock", action = "Index", id = UserDAO.Instance.getIDbyUserName(model.UserName) }));
                }
                else
                {
                    ModelState.AddModelError("", "Login Failed");
                }
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult SendPasswordToEmail()
        {
            return View("");
        }
        [HttpPost]
        public ActionResult SendPasswordToEmailPost(string email)
        {
            if (UserDAO.Instance.IsExistEmail(email))
            {
                // Cấu hình thông tin gmail
                var mail = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("dat6499proo@gmail.com", "01868390344"),
                    EnableSsl = true
                };

                //Tạo email
                var message = new MailMessage();
                message.From = new MailAddress("dat6499proo@gmail.com");
                message.ReplyToList.Add("dat6499proo@gmail.com");
                message.To.Add(new MailAddress(email));
                message.Subject = "This is your Password";
                message.Body = UserDAO.Instance.GetRealPasswordByEmail(email);

                //Gửi email
                mail.Send(message);
                //Xác nhận gửi Mail thành công
                ViewBag.SendSuccess = true;
                return View("Index");
            }
            return View("EmailError");
        }
        public ActionResult SignIn()
        {
            return View();
        }
        public ActionResult CreateNewStaff(string Username, string Email, string Password)
        {
            if (!UserDAO.Instance.IsExistUserName(Username) && !UserDAO.Instance.IsExistEmail(Email))
            {
                int id = UserDAO.Instance.CreateFirstStaff(Username, Password, Email);
                if (id > 0)
                    return View("Index");
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session["TICKETSTAFF_USER_SESSION"] = null;
            return RedirectToAction("Index", "Login");
        }
    }
}