using LemonCat.Common;
using LemonCat.Models.DAO;
using LemonCat.Models.EF;
using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LemonCat.Areas.Manager.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        // GET: Admin/Home
        public ActionResult Index()
        {
            ViewBag.TongMovie = YearDataDAO.Instance.MovieCount();
            ViewBag.TongTV = YearDataDAO.Instance.TVCount();
            ViewBag.TongReview = YearDataDAO.Instance.ReviewCount();
            ViewBag.TongNews = YearDataDAO.Instance.NewsCount();
            ViewBag.Person = YearDataDAO.Instance.PersonCount();
            ViewBag.TongUser = YearDataDAO.Instance.UserCount();
            return View();
        }
        public ActionResult BarChart()
        {
            int month = int.Parse(DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-").Split('-')[1]);
            int year = int.Parse(DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-").Split('-')[2]);
            var chart = YearDataDAO.Instance.GetChart(month, year);
            return Json(chart, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PieChart()
        {
            int month = int.Parse(DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-").Split('-')[1]);
            int year = int.Parse(DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-").Split('-')[2]);
            var chart = YearDataDAO.Instance.GetData(month, year);
            return Json(chart, JsonRequestBehavior.AllowGet);
        }
    }
}