using System;
using LemonCat.Common;
using LemonCat.Models.DAO;
using LemonCat.Models.EF;
using Model.DAO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LemonCat.Areas.ClientDVD.Controllers
{
    public class DVDController : Controller
    {
        // GET: ClientDVD/DVD
        public ActionResult Index()
        {
            if (Session[CommonConstaints.CLIENT_USER_SESSION] != null)
            {
                var result = DVDDAO.Instance.GetAllDVD();
                List<PHIM> ListFlim = new List<PHIM>();
                foreach (var item in result)
                {
                    ListFlim.Add(MovieDAO.Instance.GetMovie((int)item.MaPhim));
                }
                ViewBag.ListFlim = ListFlim;
                return View(result);
            }
            return RedirectToAction("Login", "Login");
        }
        public ActionResult ViewDetail(int id)
        {
            var result = DVDDAO.Instance.GetDVDByID(id);
            var flim = MovieDAO.Instance.GetMovie((int)result.MaPhim);
            ViewBag.Image = MovieDAO.Instance.GetSomeImageMovieByMovieID(flim.MaPhim);
            ViewBag.Price = result.Gia;
            ViewBag.Score = MovieDAO.Instance.GetLemonCatScoreByMovieID(flim.MaPhim);
            ViewBag.CountReview = FlimDAO.Instance.CountReviewByFlimID(flim.MaPhim);
            ViewBag.MaDVD = result.MaDVD;
            var listreview = FlimDAO.Instance.GetReviewByFlimID(flim.MaPhim);
            ViewBag.Review = listreview;
            List<string> Avata = new List<string>();
            List<string> Name = new List<string>();
            foreach (var item in listreview)
            {
                Avata.Add(UserDAO.Instance.GetAvataByID((int)item.MaTK));
                Name.Add(UserDAO.Instance.GetNameByID((int)item.MaTK));
            }
            ViewBag.Name = Name;
            ViewBag.Avata = Avata;
            return View(flim);
        }
        [HttpGet]
        public ActionResult ShowBill(int id)
        {
            //id->billID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] != null)
            {
                ViewBag.ChildBill = DVDDAO.Instance.GetChildBillByBillID(id);
                return View(DVDDAO.Instance.GetBillByID(id));
            }
            return RedirectToAction("Login", "Login");
        }
        [HttpPost]
        public ActionResult AddCart(int id)
        {
            //id->DVDID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] != null)
            {
                bool result = DVDDAO.Instance.AddCart(id, (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID);
                return Json(new
                {
                    status = result
                });
            }
            return RedirectToAction("Login", "Login");
        }
        [HttpGet]
        public ActionResult ManagerBag()
        {
            if (Session[CommonConstaints.CLIENT_USER_SESSION] != null)
            {
                int account = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
                var result = DVDDAO.Instance.GetBagByUserID(account);
                List<PHIM> ListFlim = new List<PHIM>();
                int TongTien = 0;
                foreach (var item in result)
                {
                    ListFlim.Add(MovieDAO.Instance.GetMovie(DVDDAO.Instance.GetFlimIDByDVDID((int)item.MaDVD)));
                    if (item.Status == true)
                    {
                        TongTien += (int)item.Gia * (int)item.SoLuong;
                    }
                }
                ViewBag.TongTien = TongTien;
                ViewBag.ListFlim = ListFlim;
                return View(DVDDAO.Instance.GetBagByUserID(account));
            }
            return RedirectToAction("Login", "Login");
        }
        [HttpPost]
        public JsonResult BagItemDelete(int id)
        {
            //id->BagItemID
            bool result = DVDDAO.Instance.BagItemDeleteByID(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpGet]
        public ActionResult BillManager()
        {
            if (Session[CommonConstaints.CLIENT_USER_SESSION] != null)
            {
                int id = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
                var list = DVDDAO.Instance.GetAllBillByUserID(id);
                List<string> Avata = new List<string>();
                List<string> Name = new List<string>();
                foreach (var item in list)
                {
                    Avata.Add(UserDAO.Instance.GetAvataByID((int)item.MaTK));
                    Name.Add(UserDAO.Instance.GetNameByID((int)item.MaTK));
                }
                ViewBag.Avata = Avata;
                ViewBag.Name = Name;
                return View(list);
            }
            return RedirectToAction("Login", "Login");
        }
        [HttpPost]
        public JsonResult ChangeBagItemStatus(int id)
        {
            //id->BagItemID
            bool result = DVDDAO.Instance.ChangeBagItemStatusByItemID(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpPost]
        public JsonResult ChangeAmountBagItem(int id, int? amount)
        {
            //id->BagItemID
            bool result = DVDDAO.Instance.ChangeAmountBagItemByItemID(id, amount);
            return Json(new
            {
                status = result
            });
        }
        [HttpPost]
        public ActionResult SubmitBag(string note)
        {
            if (note == "")
            {
                note = "Nothing to note";
            }
            int account = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
            bool result = DVDDAO.Instance.SubmitBag(account, note, DateTime.Now.ToString("dd/MM/yyyy"));
            return RedirectToAction("ManagerBag", "DVD");
        }
    }
}