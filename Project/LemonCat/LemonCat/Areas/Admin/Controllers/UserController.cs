using LemonCat.Areas.Admin.Models;
using LemonCat.Common;
using LemonCat.Models.EF;
using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace LemonCat.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        // GET: Admin/User
        public ActionResult Index()
        {
            ViewBag.PositionList = UserDAO.Instance.GetAllNameOfPositionByIndex();
            return View(UserDAO.Instance.GetAllUser());
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool statuss = UserDAO.Instance.Delete(id);
            return Json(new
            {
                status = statuss
            });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.PositionListName = UserDAO.Instance.GetAllPosition();
            return View(UserDAO.Instance.GetByID(id));
        }
        [HttpPost]
        public ActionResult Edit(FormCollection form, HttpPostedFileBase AnhDaiDien)
        {
            TAIKHOAN user = new TAIKHOAN();
            user.TenTK = form["TenTk"];
            user.MatKhau = form["MatKhau"];
            user.FirstName = form["FirstName"];
            user.LastName = form["LastName"];
            user.Email = form["Email"];
            user.DiaChi = form["DiaChi"];
            user.Phone = form["Phone"];
            user.CMND = form["CMND"];
            user.ChucVu = int.Parse(form["ChucVu"]);
            if (form["GioiTinh"] == "on")
                user.GioiTinh = true;
            else
                user.GioiTinh = false;
            if (form["KichHoat"] == "on")
                user.KichHoat = true;
            else
                user.KichHoat = false;

            if (AnhDaiDien != null)
            {
                // Get file name
                var filename = Path.GetFileName(AnhDaiDien.FileName);
                // Path
                var path = Path.Combine(Server.MapPath("~/Avata"), filename);
                // Check Exist file
                if (System.IO.File.Exists(path))
                {
                    // If File Existed -> Add "_" before filename and save again until File not Exits 
                    while (System.IO.File.Exists(path))
                    {
                        filename = "_" + filename;
                        path = Path.Combine(Server.MapPath("~/Avata"), filename);
                    }
                }
                user.AnhDaiDien = "/Avata/" + Path.GetFileName(AnhDaiDien.FileName);
                AnhDaiDien.SaveAs(path);
            }
            int result = UserDAO.Instance.Update(user);
            return RedirectToAction("Index", "User");
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.PositionListName = UserDAO.Instance.GetAllPosition();
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection form, HttpPostedFileBase AnhDaiDien)
        {
            TAIKHOAN user = new TAIKHOAN();
            user.TenTK = form["TenTk"];
            user.MatKhau = form["MatKhau"];
            user.FirstName = form["FirstName"];
            user.LastName = form["LastName"];
            user.Email = form["Email"];
            user.DiaChi = form["DiaChi"];
            user.Phone = form["Phone"];
            user.CMND = form["CMND"];
            user.ChucVu = UserDAO.Instance.GetPositionByName(form["ChucVu"]);
            if (form["GioiTinh"] == "on")
                user.GioiTinh = true;
            else
                user.GioiTinh = false;
            if (form["KichHoat"] == "on")
                user.KichHoat = true;
            else
                user.KichHoat = false;
            
            if (AnhDaiDien == null)
            {
                user.AnhDaiDien = "/Avata/profile_av.jpg";
            }
            else
            {
                // Get file name
                var filename = Path.GetFileName(AnhDaiDien.FileName);
                // Path
                var path = Path.Combine(Server.MapPath("~/Avata"), filename);
                // Check Exist file
                if (System.IO.File.Exists(path))
                {
                    // If File Existed -> Add "_" before filename and save again until File not Exits 
                    while (System.IO.File.Exists(path))
                    {
                        filename = "_" + filename;
                        path = Path.Combine(Server.MapPath("~/Avata"), filename);
                    }
                }
                user.AnhDaiDien = "/Avata/" + filename;
                AnhDaiDien.SaveAs(path);
            }
            int id = UserDAO.Instance.Insert(user);
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var result = UserDAO.Instance.ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }
        public ActionResult Logout()
        {
            Session["USER_SESSION"] = null;
            return Json(new
            {
                status = true
            });
        }
    }
}