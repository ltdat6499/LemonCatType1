using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LemonCat.Areas.Manager.Controllers
{
    public class LockController : Controller
    {
        // GET: Admin/Lock
        public ActionResult Index(int id)
        {
            ViewBag.image = UserDAO.Instance.GetAvataByID(id);
            ViewBag.name = UserDAO.Instance.GetNameByID(id);
            return View();
        }
    }
}