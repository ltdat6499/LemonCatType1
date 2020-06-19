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
    public class DVDController : BaseController
    {
        // GET: Admin/DVD
        public ActionResult Index()
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
        [HttpGet]
        public ActionResult Create()
        {
            var list = FlimDAO.Instance.GetAllFlim();
            List<PHIM> FlimList = new List<PHIM>();
            foreach (var item in list)
            {
                if (!DVDDAO.Instance.IsExist(item.MaPhim))
                {
                    FlimList.Add(item);
                }
            }
            if (FlimList.Count == 0)
            {
                RedirectToAction("Index", "Error");
            }
            ViewBag.FlimList = FlimList;
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            int flimID = int.Parse(form["Phim"]);
            int Disks = int.Parse(form["Disks"]);
            int Price = int.Parse(form["Price"]);
            int Count = int.Parse(form["Count"]);
            DVDDAO.Instance.InsertDVD(flimID, Disks, Price, Count, DateTime.Now.ToString("dd/MM/yyyy"));
            return RedirectToAction("Index", "DVD");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //id->DVDID
            ViewBag.FlimList = FlimDAO.Instance.GetAllFlim();
            return View(DVDDAO.Instance.GetDVDByID(id));
        }
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            int flimID = int.Parse(form["Phim"]);
            int Disks = int.Parse(form["Disks"]);
            int Price = int.Parse(form["Price"]);
            int Count = int.Parse(form["Count"]);
            int DVDID = int.Parse(form["ID"]);
            DVDDAO.Instance.UpdateDVD(DVDID, flimID, Disks, Price, Count, DateTime.Now.ToString("dd/MM/yyyy"));
            return RedirectToAction("Index", "DVD");
        }
        [HttpPost]
        public JsonResult ChangeStatus(int id)
        {
            //id->DVDID
            var result = DVDDAO.Instance.ChangeStatus(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            //id->DVDID
            var result = DVDDAO.Instance.Delete(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpGet]
        public ActionResult BillManager()
        {
            var list = DVDDAO.Instance.GetAllBill();
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



        [HttpPost]
        public JsonResult ChangeStatusBill(int id)
        {
            //id->BillID
            int result = DVDDAO.Instance.ChangeStatusBill(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpGet]
        public ActionResult ShowBill(int id)
        {
            //id->billID
            ViewBag.ChildBill = DVDDAO.Instance.GetChildBillByBillID(id);
            return View(DVDDAO.Instance.GetBillByID(id));
        }
        [HttpPost]
        public ActionResult AcceptBill(int id)
        {
            DVDDAO.Instance.AcceptBillByBillID(id);
            return RedirectToAction("BillManager", "DVD");
        }



        [HttpGet]
        public ActionResult ProductView()
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
        [HttpPost]
        public JsonResult AddCart(int id)
        {
            //id->DVDID
            bool result = DVDDAO.Instance.AddCart(id, (Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID);
            return Json(new
            {
                status = result
            });
        }
        [HttpGet]
        public ActionResult ManagerBag()
        {
            int account = (Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID;
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
            int account = (Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID;
            bool result = DVDDAO.Instance.SubmitBag(account, note, DateTime.Now.ToString("dd/MM/yyyy"));
            if (result)
            {
                return RedirectToAction("ManagerBag", "DVD");
            }
            return Json(new
            {
                status = result
            });
        }
    }
}