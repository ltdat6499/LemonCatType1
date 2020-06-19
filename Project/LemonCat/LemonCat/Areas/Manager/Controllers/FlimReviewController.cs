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
    public class FlimReviewController : BaseController
    {
        // GET: Admin/FlimReview
        public ActionResult Index()
        {
            var list = FlimDAO.Instance.GetAllFlim();
            List<int> UserScore = new List<int>();
            List<int> LemonCat = new List<int>();
            foreach (var item in list)
            {
                UserScore.Add(MovieDAO.Instance.GetUserScoreByMovieID(item.MaPhim));
                LemonCat.Add(MovieDAO.Instance.GetLemonCatScoreByMovieID(item.MaPhim));
            }
            ViewBag.UserScore = UserScore;
            ViewBag.LemonCat = LemonCat;
            return View(list);
        }
        [HttpGet]
        public ActionResult WriteReview(int id)
        {
            //id->FlimID
            ViewBag.ID = id;
            return View();
        }
        public string DecodeMarkCode(string markcode)
        {
            markcode = markcode.Replace("&lt;", "<");
            markcode = markcode.Replace("&gt;", ">");
            return markcode;
        }
        [HttpPost]
        public ActionResult WriteReview(FormCollection form)
        {
            //id->FlimID
            int account = (Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID;
            int flimid = int.Parse(form["ID"]);
            if (FlimDAO.Instance.IsReview(account, flimid))
            {
                return RedirectToAction("Index", "Error");
            }
            string NoiDungNgan = form["NoiDungNgan"];
            string NoiDungBinhLuan = form["NoiDungBinhLuan"];
            NoiDungBinhLuan = DecodeMarkCode(NoiDungBinhLuan);
            float Rating = float.Parse(form["RadioValue"]);
            int finalScore = (int)(Rating * 2) * 10;

            int id = FlimDAO.Instance.InsertReview(flimid, NoiDungBinhLuan, NoiDungNgan, finalScore, DateTime.Now.ToString("dd/MM/yyyy"), account);
            return RedirectToAction("Index", "FlimReview");
        }
        [HttpGet]
        public ActionResult ManagerReview(int id)
        {
            //id->flimID
            ViewBag.ID = id;
            var result = FlimDAO.Instance.GetReviewByFlimID(id);
            List<string> AvataOwner = new List<string>();
            List<string> NameOwner = new List<string>();
            List<bool> MarkNews = new List<bool>();
            foreach (var item in result)
            {
                MarkNews.Add(FlimDAO.Instance.GetReviewMarkByUserID((Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID, item.MaBaiViet));
                AvataOwner.Add(UserDAO.Instance.GetAvataByID((int)item.MaTK));
                NameOwner.Add(UserDAO.Instance.GetNameByID((int)item.MaTK));
            }
            ViewBag.MarkNews = MarkNews;
            ViewBag.AvataOwner = AvataOwner;
            ViewBag.NameOwner = NameOwner;
            return View(result);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            // id->Reviewid
            FlimDAO.Instance.Delete(id);
            return Json(new
            {
                status = true
            });
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            //id->ReviewID
            return View(FlimDAO.Instance.GetReviewByID(id));
        }
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            //id->ReviewID
            int id = int.Parse(form["ID"]);
            string NoiDungNgan = form["NoiDungNgan"];
            string NoiDungBinhLuan = form["NoiDungBinhLuan"];
            NoiDungBinhLuan = DecodeMarkCode(NoiDungBinhLuan);
            float Rating = float.Parse(form["RadioValue"]);
            int finalScore = (int)(Rating * 2) * 10;

            FlimDAO.Instance.UpdateReview(id, NoiDungBinhLuan, NoiDungNgan, finalScore, DateTime.Now.ToString("dd/MM/yyyy"));
            return RedirectToAction("Index", "FlimReview");
        }
        [HttpPost]
        public JsonResult ChangeStatusReview(int id)
        {
            // id->newsid
            bool status = FlimDAO.Instance.ChangeStatusReviewByID(id);
            return Json(new
            {
                status = status
            });
        }
        [HttpGet]
        public ActionResult ShowLikeNews(int id)
        {
            //id->ReviewID
            var list = FlimDAO.Instance.GetListReviewScoreByReviewID(id);
            List<bool> Mark = new List<bool>();
            List<TAIKHOAN> result = new List<TAIKHOAN>();
            foreach (var item in list)
            {
                result.Add(UserDAO.Instance.GetByID((int)item.MaTK));
                Mark.Add((bool)item.Mark);
            }
            ViewBag.Mark = Mark;
            return View(result);
        }
        [HttpPost]
        public JsonResult LikeNews(int id)
        {
            //id->NewsID
            bool status = FlimDAO.Instance.LikeReviewByUserIDAndReviewID(id, (Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID);
            return Json(new
            {
                status = status,
                mark = FlimDAO.Instance.GetReviewByID(id).DiemDanhGia
            });
        }
        [HttpGet]
        public ActionResult ManagerComment(int id)
        {
            //id->NewsID
            var ListRootComment = FlimDAO.Instance.GetListRootCommentByReviewID(id);
            List<List<BAIVIET_CHILDCOMMENT>> ListChildComment = new List<List<BAIVIET_CHILDCOMMENT>>();
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
                ListChildComment.Add(FlimDAO.Instance.GetListChildByRootID(item.ID));
                MarkComment.Add(FlimDAO.Instance.GetRootCommentMarkByUserID((Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID, item.ID));
                CountChildComment.Add(FlimDAO.Instance.CountChildByRootID(item.ID));
                CountLikeRoot.Add(FlimDAO.Instance.CountLikeRootByRootID(item.ID));
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
        public JsonResult WriteComment(int id, string content)
        {
            //id->ReviewID
            int account = (Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID;
            int result = FlimDAO.Instance.WriteComment(id, content, account, DateTime.Now.ToString("dd/MM/yyyy"));
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
            int account = (Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID;
            int result = FlimDAO.Instance.WriteChildComment(id, content, account, DateTime.Now.ToString("dd/MM/yyyy"));
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
        public JsonResult DeleteChildComment(int id)
        {
            // id->Childid
            FlimDAO.Instance.DeleteChildByChildID(id);
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult DeleteRootComment(int id)
        {
            // id->rootid
            FlimDAO.Instance.DeleteRootByRootID(id);
            return Json(new
            {
                status = true
            });
        }
        [HttpPost]
        public JsonResult ChangeRootStatus(int id)
        {
            // id->rootid
            bool result = FlimDAO.Instance.ChangeStatusRootByRootID(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpPost]
        public JsonResult ChangeChildStatus(int id)
        {
            // id->rootid
            bool result = FlimDAO.Instance.ChangeStatusChildByChildID(id);
            return Json(new
            {
                status = result
            });
        }
        [HttpPost]
        public JsonResult LikeRoot(int id)
        {
            //id->RootID
            bool status = FlimDAO.Instance.LikeRootByUserIDAndRootID(id, (Session[CommonConstaints.MANAGER_USER_SESSION] as UserLogin).UserID);
            return Json(new
            {
                status = status
            });
        }
    }
}