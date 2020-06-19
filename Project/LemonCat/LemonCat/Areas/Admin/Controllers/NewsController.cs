using LemonCat.Common;
using LemonCat.Models.DAO;
using LemonCat.Models.EF;
using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LemonCat.Areas.Admin.Controllers
{
    public class NewsController : BaseController
    {
        // GET: Admin/News
        public ActionResult Index()
        {
            var result = NewsDAO.Instance.GetAllNews();
            List<List<string>> AvataFlim = new List<List<string>>();
            List<List<string>> AvataPerson = new List<List<string>>();
            List<string> AvataOwner = new List<string>();
            List<string> NameOwner = new List<string>();
            List<bool> MarkNews = new List<bool>();
            foreach (var item in result)
            {
                var listflimnew = NewsDAO.Instance.GetAllFlimByNewsID(item.MaNew);
                List<string> flimresult = new List<string>();
                foreach (var itemflim in listflimnew)
                {
                    flimresult.Add(MovieDAO.Instance.GetMovieAvataByMovieID((int)itemflim.MaPhim));
                }
                AvataFlim.Add(flimresult);

                var listpersonnew = NewsDAO.Instance.GetAllPersonByNewsID(item.MaNew);
                List<string> personresult = new List<string>();
                foreach (var itemperson in listpersonnew)
                {
                    personresult.Add(PersonDAO.Instance.GetAvataByID((int)itemperson.MaDienVien));
                }
                AvataPerson.Add(personresult);
                MarkNews.Add(NewsDAO.Instance.GetNewsMarkByUserID((Session[CommonConstaints.USER_SESSION] as UserLogin).UserID, item.MaNew));
                AvataOwner.Add(UserDAO.Instance.GetAvataByID((int)item.MaTK));
                NameOwner.Add(UserDAO.Instance.GetNameByID((int)item.MaTK));
            }
            ViewBag.AvataFlim = AvataFlim;
            ViewBag.AvataPerson = AvataPerson;
            ViewBag.MarkNews = MarkNews;
            ViewBag.AvataOwner = AvataOwner;
            ViewBag.NameOwner = NameOwner;
            return View(result);
        }
        public ActionResult Create()
        {
            ViewBag.FlimList = FlimDAO.Instance.GetAllFlim();
            ViewBag.PersonList = PersonDAO.Instance.GetAllPerson();
            ViewBag.TypeList = NewsDAO.Instance.GetAllType();
            return View();
        }
        public string DecodeMarkCode(string markcode)
        {
            markcode = markcode.Replace("&lt;", "<");
            markcode = markcode.Replace("&gt;", ">");
            return markcode;
        }
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            int id = (Session[CommonConstaints.USER_SESSION] as UserLogin).UserID;
            string tuade = form["TuaDe"];
            string noidungngan = form["NoiDungNgan"];
            string noidung = form["NoiDung"];
            noidung = DecodeMarkCode(noidung);
            int loainew = int.Parse(form["LoaiNew"]);
            int NewsID = NewsDAO.Instance.CreateNew(tuade, id, loainew, noidung, noidungngan, DateTime.Now.ToString("dd/MM/yyyy").Replace('/', '-'));
            
            if (form["Phim"] != null && NewsID > 0)
            {
                string[] Phim = form["Phim"].Split(',');
                foreach (string item in Phim)
                {
                    NewsDAO.Instance.InsertFlimToNews(NewsID, int.Parse(item));
                }
            }
            
            if (form["DienVien"] != null && NewsID > 0)
            {
                string[] DienVien = form["DienVien"].Split(',');
                foreach (string item in DienVien)
                {
                    NewsDAO.Instance.InsertPersontoNews(NewsID, int.Parse(item));
                }
            }

            return RedirectToAction("Index", "News");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //id->newID
            ViewBag.TypeList = NewsDAO.Instance.GetAllType();
            ViewBag.FlimList = FlimDAO.Instance.GetAllFlim();
            ViewBag.PersonList = PersonDAO.Instance.GetAllPerson();
            ViewBag.Phim = NewsDAO.Instance.GetAllFlimByNewsID(id);
            ViewBag.DienVien = NewsDAO.Instance.GetAllPersonByNewsID(id);
            return View(NewsDAO.Instance.GetNews(id));
        }
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            //id->newID
            string tuade = form["TuaDe"];
            string noidungngan = form["NoiDungNgan"];
            string noidung = form["NoiDung"];
            noidung = DecodeMarkCode(noidung);
            int loainew = int.Parse(form["LoaiNew"]);
            int id = int.Parse(form["ID"]);
            NewsDAO.Instance.EditNew(id, tuade, loainew, noidung, noidungngan, DateTime.Now.ToString("dd/MM/yyyy").Replace('/', '-'));

            
            if (form["Phim"] != null && id > 0)
            {
                string[] Phim = form["Phim"].Split(',');
                NewsDAO.Instance.RemoveFlimNews(id);
                foreach (string item in Phim)
                {
                    NewsDAO.Instance.InsertFlimToNews(id, int.Parse(item));
                }
            }

            if (form["DienVien"] != null && id > 0)
            {
                string[] DienVien = form["DienVien"].Split(',');
                NewsDAO.Instance.RemovePersontoNews(id);
                foreach (string item in DienVien)
                {
                    NewsDAO.Instance.InsertPersontoNews(id, int.Parse(item));
                }
            }
            return RedirectToAction("Index", "News");
        }
        [HttpPost]
        public JsonResult ChangeStatusNews(int id)
        {
            // id->newsid
            bool status = NewsDAO.Instance.ChangeStatusNewsByNewsID(id);
            return Json(new
            {
                status = status
            });
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            // id->Newsid
            NewsDAO.Instance.Delete(id);
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult DeleteChildComment(int id)
        {
            // id->Childid
            NewsDAO.Instance.DeleteChildByChildID(id);
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult DeleteRootComment(int id)
        {
            // id->rootid
            NewsDAO.Instance.DeleteRootByRootID(id);
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult ChangeStatusRoot(int id)
        {
            // id->rootid
            bool result = NewsDAO.Instance.ChangeStatusRootByRootID(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpPost]
        public JsonResult ChangeStatusChild(int id)
        {
            // id->rootid
            bool result = NewsDAO.Instance.ChangeStatusChildByChildID(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpPost]
        public JsonResult WriteComment(int id, string content)
        {
            //id->NewsID
            int account = (Session[CommonConstaints.USER_SESSION] as UserLogin).UserID;
            int result = NewsDAO.Instance.WriteComment(id, content, account, DateTime.Now.ToString("dd/MM/yyyy").Replace('/', '-'));
            string avata = UserDAO.Instance.GetAvataByID(account);
            string name = UserDAO.Instance.GetNameByID(account);
            return Json(new
            {
                IDComment = result,
                Avata = avata,
                Name = name,
                Date = DateTime.Now.ToString("dd/MM/yyyy")
            });
        }
        [HttpPost]
        public JsonResult WriteChildComment(int id, string content)
        {
            //id->RootID
            int account = (Session[CommonConstaints.USER_SESSION] as UserLogin).UserID;
            int result = NewsDAO.Instance.WriteChildComment(id, content, account, DateTime.Now.ToString("dd/MM/yyyy").Replace('/', '-'));
            string avata = UserDAO.Instance.GetAvataByID(account);
            string name = UserDAO.Instance.GetNameByID(account);
            return Json(new
            {
                IDComment = result,
                Avata = avata,
                Name = name,
                Date = DateTime.Now.ToString("dd/MM/yyyy")
            });
        }
        [HttpGet]
        public ActionResult ManagerComment(int id)
        {
            //id->NewsID
            var ListRootComment = NewsDAO.Instance.GetListRootCommentByNewsID(id);
            List<List<ChildCommentNew>> ListChildComment = new List<List<ChildCommentNew>>();
            List<string> AvataComment = new List<string>();
            List<string> NameComment = new List<string>();
            List<List<string>> AvataChildComment = new List<List<string>>();
            List<List<string>> NameChildComment = new List<List<string>>();
            List<int> CountChildComment = new List<int>();
            List<int> CountLikeRoot = new List<int>();
            List<bool> MarkComment = new List<bool>();
            foreach (var item in ListRootComment)
            {
                AvataComment.Add(UserDAO.Instance.GetAvataByID((int)item.MaTK));
                NameComment.Add(UserDAO.Instance.GetNameByID((int)item.MaTK));
                ListChildComment.Add(NewsDAO.Instance.GetListChildByRootID(item.ID));
                MarkComment.Add(NewsDAO.Instance.GetRootCommentMarkByUserID((Session[CommonConstaints.USER_SESSION] as UserLogin).UserID, item.ID));
                CountChildComment.Add(NewsDAO.Instance.CountChildByRootID(item.ID));
                CountLikeRoot.Add(NewsDAO.Instance.CountLikeRootByRootID(item.ID));
            }
            foreach (var item in ListChildComment)
            {
                List<string> childavata = new List<string>();
                List<string> childname = new List<string>();
                foreach (var item2 in item)
                {
                    childavata.Add(UserDAO.Instance.GetAvataByID(((int)item2.MaTK)));
                    childname.Add(UserDAO.Instance.GetNameByID(((int)item2.MaTK)));
                }
                AvataChildComment.Add(childavata);
                NameChildComment.Add(childname);
            }
            ViewBag.ListCommentThree = ListChildComment;
            ViewBag.AvataComment = AvataComment;
            ViewBag.NameComment = NameComment;
            ViewBag.MarkComment = MarkComment;
            ViewBag.AvataChildComment = AvataChildComment;
            ViewBag.NameChildComment = NameChildComment;
            ViewBag.CountChildComment = CountChildComment;
            ViewBag.CountLikeRoot = CountLikeRoot;
            ViewBag.ID = id;
            return View(ListRootComment);
        }

        [HttpPost]
        public JsonResult LikeRoot(int id)
        {
            //id->RootID
            bool status = NewsDAO.Instance.LikeRootByUserIDAndRootID(id, (Session[CommonConstaints.USER_SESSION] as UserLogin).UserID);
            return Json(new
            {
                status = status
            });
        }
        [HttpPost]
        public JsonResult LikeNews(int id)
        {
            //id->NewsID
            bool status = NewsDAO.Instance.LikeNewsByUserIDAndNewsID(id, (Session[CommonConstaints.USER_SESSION] as UserLogin).UserID);
            return Json(new
            {
                status = status,
                mark = NewsDAO.Instance.GetNews(id).DiemDanhGia
            });
        }
        [HttpGet]
        public ActionResult ShowLikeNews(int id)
        {
            //id->NewsID
            var list = NewsDAO.Instance.GetListNewsScoreByNewsID(id);
            List<bool> Mark = new List<bool>();
            List<TAIKHOAN> result = new List<TAIKHOAN>();
            foreach (var item in list)
            {
                result.Add(UserDAO.Instance.GetByID((int)item.MaTK));
                Mark.Add((bool)item.Score);
            }
            ViewBag.Mark = Mark;
            return View(result);
        }
    }
}