using LemonCat.Common;
using LemonCat.Models.DAO;
using LemonCat.Models.EF;
using Model.DAO;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LemonCat.Areas.Client.Models
{
    public class HomeController : Controller
    {
        // GET: Client/Home
        public ActionResult Index()
        {
            var ListPanel = MovieDAO.Instance.GetAllMovie();
            List<PHIM> panneltemp = new List<PHIM>();
            for (int i = ListPanel.Count - 1; i >= ListPanel.Count - 6; i--)
            {
                panneltemp.Add(ListPanel[i]);
            }
            List<List<string>> GenresList = new List<List<string>>();
            List<string> Video = new List<string>();
            List<string> Image = new List<string>();
            foreach (var item in panneltemp)
            {
                GenresList.Add(MovieDAO.Instance.GetGenreNameMovieByMovieID((int)item.MaPhim));
                Video.Add(MovieDAO.Instance.GetMovie((int)item.MaPhim).Trailer);
                Image.Add(MovieDAO.Instance.GetSomeImageMovieByMovieID((int)item.MaPhim)[0].Anh);
            }
            ViewBag.Video = Video;
            ViewBag.Image = Image;

            ViewBag.GenresList = GenresList;
            ViewBag.ListPanel = panneltemp;

            ViewBag.SlideCMS = MovieDAO.Instance.ComingSoon(1, 5, "");
            ViewBag.SlideTopM = MovieDAO.Instance.CertifiedMovies(1, 5, "");
            ViewBag.SlideMostReview = MovieDAO.Instance.TopBox(1, 5, "");



            var ListPanelTV = TVShowDAO.Instance.GetAllTVShow();
            List<PHIM> tempTVS = new List<PHIM>();
            for (int i = ListPanelTV.Count - 1; i >= ListPanelTV.Count - 3; i--)
            {
                tempTVS.Add(ListPanelTV[i]);
            }
            ViewBag.tempTVS = tempTVS;

            ViewBag.TVCMS = TVShowDAO.Instance.ComingSoon(1, 6, "");
            ViewBag.TVTOP = TVShowDAO.Instance.Fresh(1, 6, "");

            ViewBag.Star = PersonDAO.Instance.GetAllPersonStar(1, 4, "");


            var test = NewsDAO.Instance.GetAllNews();
            ViewBag.LastNew = test[test.Count - 1];
            return View();
        }
        public ActionResult Movie(int id, int page = 1, int? pagesize = 5)
        {
            if (Session["CLIENT_USER_SESSION"] != null)
            {
                MovieDAO.Instance.AddDataSet(id, (Session["CLIENT_USER_SESSION"] as UserLogin).UserID);
            }
            var Genres = MovieDAO.Instance.GetGenreMovieByMovieID(id);
            List<string> GenresList = new List<string>();
            foreach (var item in Genres)
            {
                GenresList.Add(MovieDAO.Instance.GetGenreByGenreID((int)item.MaGenre));
            }
            var Character = MovieDAO.Instance.GetCastMovieByMovieID(id);
            List<DIENVIEN> ListCast = new List<DIENVIEN>();
            List<DIENVIEN> StarList = new List<DIENVIEN>();
            foreach (var item in Character)
            {
                ListCast.Add(PersonDAO.Instance.GetByID((int)item.MaDienVien));
                if (ListCast[ListCast.Count - 1].Star == true)
                {
                    StarList.Add(PersonDAO.Instance.GetByID((int)item.MaDienVien));
                }

            }
            var Director = MovieDAO.Instance.GetDirectorMovieByMovieID(id);
            List<DIENVIEN> ListDirector = new List<DIENVIEN>();
            foreach (var item in Director)
            {
                ListDirector.Add(PersonDAO.Instance.GetByID((int)item.MaDienVien));
            }
            var Writtent = MovieDAO.Instance.GetWrittenMovieByMovieID(id);
            List<DIENVIEN> ListWrittent = new List<DIENVIEN>();
            foreach (var item in Writtent)
            {
                ListWrittent.Add(PersonDAO.Instance.GetByID((int)item.MaDienVien));
            }
            ViewBag.Character = Character;
            ViewBag.Cast = ListCast;
            ViewBag.Score = MovieDAO.Instance.GetScoreByMovieID(id);
            ViewBag.Image = MovieDAO.Instance.GetSomeImageMovieByMovieID(id);
            ViewBag.FullImage = MovieDAO.Instance.GetAllImageMovieByMovieID(id);
            ViewBag.CountReview = FlimDAO.Instance.CountReviewByFlimID(id);
            var TopReview = FlimDAO.Instance.GetTopReviewByFlimID(id);
            List<string> TopReviewName = new List<string>();
            List<string> TopReviewAvata = new List<string>();
            foreach (var item in TopReview)
            {
                TopReviewName.Add(UserDAO.Instance.GetNameByID((int)item.MaTK));
                TopReviewAvata.Add(UserDAO.Instance.GetAvataByID((int)item.MaTK));
            }
            ViewBag.TopReview = TopReview;
            ViewBag.TopReviewName = TopReviewName;
            ViewBag.TopReviewAvata = TopReviewAvata;
            ViewBag.Director = ListDirector;
            ViewBag.Written = ListWrittent;
            ViewBag.Star = StarList;
            ViewBag.Genres = GenresList;
            ViewBag.Rating = MovieDAO.Instance.GetNameRatingByMovieID(id);
            ViewBag.ImageCount = MovieDAO.Instance.CountImageByMovieID(id);
            ViewBag.TagMovie = MovieDAO.Instance.GetTag(id);
            var RList = MovieDAO.Instance.FindRelativeMovie(id, page, (int)pagesize);
            List<string> RScore = new List<string>();
            List<string> RRating = new List<string>();
            List<List<DIENVIEN>> RStarList = new List<List<DIENVIEN>>();
            List<DIENVIEN> RDirector = new List<DIENVIEN>();
            foreach (var item in RList)
            {
                RScore.Add(MovieDAO.Instance.GetScoreByMovieID((int)item.MaPhim).LemonCatScore.ToString());
                RRating.Add(MovieDAO.Instance.GetNameRatingByMovieID((int)item.MaPhim));
                var tempdirector = MovieDAO.Instance.GetDirectorMovieByMovieID((int)item.MaPhim);
                RDirector.Add(PersonDAO.Instance.GetByID((int)tempdirector[0].MaDienVien));
                var listcast = MovieDAO.Instance.GetCastMovieByMovieID((int)item.MaPhim);
                List<DIENVIEN> temp = new List<DIENVIEN>();
                foreach (var item2 in listcast)
                {
                    var tempcast = PersonDAO.Instance.GetByID((int)item2.MaDienVien);
                    if (tempcast.Star == true)
                    {
                        temp.Add(PersonDAO.Instance.GetByID((int)tempcast.MaDienVien));
                    }
                }
                RStarList.Add(temp);
            }

            ViewBag.RelativeScore = RScore;
            ViewBag.RelativeMovie = RList;
            ViewBag.RelativeRating = RRating;
            ViewBag.RelativeDirector = RDirector;
            ViewBag.RStarList = RStarList;

            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pagesize;
            return View(MovieDAO.Instance.GetMovie(id));
        }
        public ActionResult Person(int id)
        {
            var result = PersonDAO.Instance.GetByID(id);
            ViewBag.Image = PersonDAO.Instance.GetSomeImagePersonByPersonID(id);
            Tuple<List<PHIM>, List<string>, List<string>, int> RMovieResult = PersonDAO.Instance.GetMovieByPersonID(id);
            ViewBag.RMovie = RMovieResult.Item1;
            ViewBag.RCharacterName = RMovieResult.Item2;
            ViewBag.RYear = RMovieResult.Item3;
            ViewBag.RMovieCount = RMovieResult.Item4;
            ViewBag.Tag = PersonDAO.Instance.GetTag(id);
            ViewBag.Image = PersonDAO.Instance.GetAllImageByID(id);
            ViewBag.CountImage = PersonDAO.Instance.CountImage(id);
            return View(result);
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            bool res = UserDAO.Instance.ClientUserLogin(username, Encryptor.MD5Hash(password));
            if (res)
            {
                var user = UserDAO.Instance.GetByUserName(username);
                var usersession = new UserLogin();
                usersession.UserID = user.MaTK;
                usersession.UserName = user.TenTK;
                usersession.AvataPath = user.AnhDaiDien;
                usersession.Name = user.LastName;
                Session.Add(CommonConstaints.CLIENT_USER_SESSION, usersession);
                return Json(new
                {
                    id = usersession.UserID,
                    status = true
                });
            }
            else if (UserDAO.Instance.ClientIsLocked(username))
            {
                return Json(new
                {
                    Lock = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false
                });
            }
        }
        [HttpGet]
        public ActionResult SendPasswordToEmail()
        {
            return View("");
        }
        [HttpGet]
        public ActionResult SendPasswordToEmailPost()
        {
            return View();
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
                return View("SendPasswordToEmailPost");
            }
            return View("Index");
        }
        public ActionResult SignIn()
        {
            return View();
        }
        public ActionResult CreateNewUser(string Username, string Email, string Password)
        {
            if (!UserDAO.Instance.SQLIsNotValidAccount(Username, Email, Password))
            {
                int id = UserDAO.Instance.CreateFirstUser(Username, Password, Email);
                if (id > 0)
                {
                    var user = UserDAO.Instance.GetByID(id);
                    var usersession = new UserLogin();
                    usersession.UserID = user.MaTK;
                    usersession.UserName = user.TenTK;
                    usersession.AvataPath = user.AnhDaiDien;
                    usersession.Name = user.LastName;
                    Session.Add(CommonConstaints.CLIENT_USER_SESSION, usersession);
                    return Json(new
                    {
                        id = usersession.UserID,
                        status = true
                    });
                }
            }
            return Json(new
            {
                status = false
            });
        }
        public ActionResult Logout()
        {
            Session["CLIENT_USER_SESSION"] = null;
            return Json(new
            {
                status = true
            });
        }
        public ActionResult UserInfo(int id)
        {
            if (Session["CLIENT_USER_SESSION"] != null)
            {
                int result = UserDAO.Instance.GetPositionByID(id);
                ViewBag.ChucVu = UserDAO.Instance.GetPositionNameByID(result);
                return View(UserDAO.Instance.GetByID(id));
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult EditInfo(FormCollection form)
        {
            int id = (Session["CLIENT_USER_SESSION"] as UserLogin).UserID;
            string FirstName = form["FirstName"];
            string LastName = form["LastName"];
            string Email = form["Email"];
            string DiaChi = form["DiaChi"];
            string Phone = form["Phone"];
            string CMND = form["CMND"];
            int result = UserDAO.Instance.SQLUpdateInfoAccount(id, Email, Phone, CMND, FirstName, LastName, DiaChi);
            // Return Number  Transaction Success in Proc in SQL
            return RedirectToAction("UserInfo", new RouteValueDictionary(new { controller = "Home", action = "UserInfo", id = id }));
        }
        public ActionResult EditAvata(HttpPostedFileBase AnhDaiDien)
        {
            int id = (Session["CLIENT_USER_SESSION"] as UserLogin).UserID;
            if (AnhDaiDien == null)
            {
                return RedirectToAction("UserInfo", "Home", id);
            }
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
            string fullpath = "/Avata/" + Path.GetFileName(AnhDaiDien.FileName);
            AnhDaiDien.SaveAs(path);
            UserDAO.Instance.UpdateAvataByID(id, fullpath);
            (Session["CLIENT_USER_SESSION"] as UserLogin).AvataPath = fullpath;
            return RedirectToAction("UserInfo", new RouteValueDictionary( new { controller = "Home", action = "UserInfo", id = id }));
        }
        public ActionResult ChangePassword(FormCollection form)
        {
            int id = (Session["CLIENT_USER_SESSION"] as UserLogin).UserID;
            string OPassword = form["OPassword"];
            string NPassword = form["NPassword"];
            string RNPassword = form["RNPassword"];
            int result =  UserDAO.Instance.SQLChangePassword(id, Encryptor.MD5Hash(OPassword), Encryptor.MD5Hash(NPassword), Encryptor.MD5Hash(RNPassword), NPassword);
            return RedirectToAction("UserInfo", new RouteValueDictionary(new { controller = "Home", action = "UserInfo", id = id }));
        }
        public ActionResult YourNews(int id, int page = 1, int? pagesize = 5)
        {
            if (Session["CLIENT_USER_SESSION"] != null)
            {
                ViewBag.Dropdown = new List<SelectListItem>(){
                     new SelectListItem() { Value="5", Text= "5 Reviews" },
                     new SelectListItem() { Value="10", Text= "10 Reviews" },
                     new SelectListItem() { Value="20", Text= "20 Reviews" },
                     new SelectListItem() { Value="25", Text= "25 Reviews" },
                     new SelectListItem() { Value="50", Text= "50 Reviews" },
                };
                var list = NewsDAO.Instance.GetNewsByUserID(id, page, (int)pagesize);
                ViewBag.Review = list;
                ViewBag.Pagesize = pagesize;
                return View(UserDAO.Instance.GetByID(id));
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult RatedMovie(int id, int page = 1, int? pagesize = 5)
        {
            if (Session["CLIENT_USER_SESSION"] != null)
            {
                ViewBag.Dropdown = new List<SelectListItem>(){
                     new SelectListItem() { Value="5", Text= "5 Reviews" },
                     new SelectListItem() { Value="10", Text= "10 Reviews" },
                     new SelectListItem() { Value="20", Text= "20 Reviews" },
                     new SelectListItem() { Value="25", Text= "25 Reviews" },
                     new SelectListItem() { Value="50", Text= "50 Reviews" },
                };
                var list = FlimDAO.Instance.GetReviewByUserIDSortByIndex(id, page, (int)pagesize);
                List<PHIM> listflim = new List<PHIM>();
                foreach (var item in list)
                {
                    listflim.Add(MovieDAO.Instance.GetMovie((int)item.MaPhim));
                }
                ViewBag.CountReview = 0;
                ViewBag.Flim = listflim;
                ViewBag.Review = list;
                ViewBag.Pagesize = pagesize;
                return View(UserDAO.Instance.GetByID(id));
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult WriteReview(int id)
        {
            //id->MovieID
            if (Session["CLIENT_USER_SESSION"] != null)
            {
                int acc = (Session["CLIENT_USER_SESSION"] as UserLogin).UserID;
                bool CheckReview = FlimDAO.Instance.IsReview(acc, id);
                if (CheckReview)
                {
                    int ReviewID = FlimDAO.Instance.GetReviewByFlimIDAndUserID(id, acc).MaBaiViet;
                    return RedirectToAction("ReviewDetail", new RouteValueDictionary(new { controller = "Home", action = "ReviewDetail", id = ReviewID }));
                }
                ViewBag.ID = id;
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult WriteReview(FormCollection form)
        {
            //id->MovieID
            if (Session["CLIENT_USER_SESSION"] != null)
            {
                int account = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
                int flimid = int.Parse(form["ID"]);
                if (FlimDAO.Instance.IsReview(account, flimid))
                {
                    int ReviewID = FlimDAO.Instance.GetReviewByFlimIDAndUserID(flimid, account).MaBaiViet;
                    return RedirectToAction("ReviewDetail", new RouteValueDictionary(new { controller = "Home", action = "ReviewDetail", id = ReviewID }));
                }
                string NoiDungNgan = form["NoiDungNgan"];
                string NoiDungBinhLuan = form["NoiDungBinhLuan"];
                NoiDungBinhLuan = DecodeMarkCode(NoiDungBinhLuan);
                float Rating = float.Parse(form["RadioValue"]);
                int finalScore = (int)(Rating * 2) * 10;

                int id = FlimDAO.Instance.InsertReview(flimid, NoiDungBinhLuan, NoiDungNgan, finalScore, DateTime.Now.ToString("dd/MM/yyyy"), account);
                return RedirectToAction("ReviewDetail", new RouteValueDictionary(new { controller = "Home", action = "ReviewDetail", id = id }));
            }
            return RedirectToAction("Index", "Home");
        }
        public string DecodeMarkCode(string markcode)
        {
            markcode = markcode.Replace("&lt;", "<");
            markcode = markcode.Replace("&gt;", ">");
            return markcode;
        }
        [HttpGet]
        public ActionResult WriteNews()
        {
            if (Session["CLIENT_USER_SESSION"] != null)
            {
                int acc = (Session["CLIENT_USER_SESSION"] as UserLogin).UserID;
                ViewBag.FlimList = FlimDAO.Instance.GetAllFlim();
                ViewBag.PersonList = PersonDAO.Instance.GetAllPerson();
                ViewBag.TypeList = NewsDAO.Instance.GetAllType();
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult WriteNews(FormCollection form)
        {
            if (Session["CLIENT_USER_SESSION"] != null)
            {
                int id = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
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
                return RedirectToAction("YourNews", new RouteValueDictionary(new { controller = "Home", action = "YourNews", id = id , page  = 1, pagesize  = 5}));
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult ReviewDetail(int id)
        {
            //id->ReviewID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var result = FlimDAO.Instance.GetReviewByID(id);
            ViewBag.Tag = FlimDAO.Instance.GetListTagByReviewID(id);
            ViewBag.Image = FlimDAO.Instance.GetImageToShowByReviewID(id);
            ViewBag.CountRoot = FlimDAO.Instance.CountRootByReviewID(id);
            ViewBag.Review = result;

            var ListRootComment = FlimDAO.Instance.GetListRootCommentByReviewIDClient(result.MaBaiViet);
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
                ListChildComment.Add(FlimDAO.Instance.GetListChildByRootIDClient(item.ID));
                MarkComment.Add(FlimDAO.Instance.GetRootCommentMarkByUserID((Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID, item.ID));
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
            ViewBag.ID = result.MaBaiViet;
            return View(ListRootComment);
        }
        [HttpPost]
        public JsonResult WriteComment(int id, string content)
        {
            //id->ReviewID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return Json(new
                {
                    IDComment = -1
                });
            }
            int account = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
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
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return Json(new
                {
                    IDComment = -1
                });
            }
            //id->RootID
            int account = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
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
        public JsonResult WriteCommentNews(int id, string content)
        {
            //id->ReviewID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return Json(new
                {
                    IDComment = -1
                });
            }
            int account = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
            int result = NewsDAO.Instance.WriteComment(id, content, account, DateTime.Now.ToString("dd/MM/yyyy"));
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
        public JsonResult WriteChildCommentNews(int id, string content)
        {
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return Json(new
                {
                    IDComment = -1
                });
            }
            //id->RootID
            int account = (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID;
            int result = NewsDAO.Instance.WriteChildComment(id, content, account, DateTime.Now.ToString("dd/MM/yyyy"));
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
        public JsonResult LikeRoot(int id)
        {
            //id->RootID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return Json(new
                {
                    Fal = true
                });
            }
            bool status = FlimDAO.Instance.LikeRootByUserIDAndRootID(id, (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID);
            return Json(new
            {
                status = status
            });
        }
        [HttpPost]
        public JsonResult LikeRootNews(int id)
        {
            //id->RootID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return Json(new
                {
                    Fal = true
                });
            }
            bool status = NewsDAO.Instance.LikeRootByUserIDAndRootID(id, (Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID);
            return Json(new
            {
                status = status
            });
        }
        [HttpGet]
        public ActionResult NewsDetail(int id)
        {
            //id->NewsID
            if (Session["CLIENT_USER_SESSION"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.News = NewsDAO.Instance.GetNews(id);

            ViewBag.Image = NewsDAO.Instance.GetImageToShowByNewsID(id);
            ViewBag.CountRoot = NewsDAO.Instance.CountRoot(id);

            var ListRootComment = NewsDAO.Instance.GetListRootCommentByNewsIDClient(id);
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
                ListChildComment.Add(NewsDAO.Instance.GetListChildByRootIDClient(item.ID));
                MarkComment.Add(NewsDAO.Instance.GetRootCommentMarkByUserID((Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID, item.ID));
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

        //Home Phase
        [HttpGet]
        public ActionResult OpeningThisMonth(int page = 1, int? pageSize = 10, string searchString = "")
        {
            List<int> Score = new List<int>();
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = MovieDAO.Instance.OpeningThisMonth(page, (int)pageSize, searchString);
            foreach (var item in result)
            {
                Score.Add(MovieDAO.Instance.GetLemonCatScoreByMovieID(item.MaPhim));
            }
            ViewBag.Score = Score;
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }
        [HttpGet]
        public ActionResult ComingSoomToTheaters(int page = 1, int? pageSize = 10, string searchString = "")
        {
            List<int> Score = new List<int>();
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = MovieDAO.Instance.ComingSoon(page, (int)pageSize, searchString);
            foreach (var item in result)
            {
                Score.Add(MovieDAO.Instance.GetLemonCatScoreByMovieID(item.MaPhim));
            }
            ViewBag.Score = Score;
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }
        [HttpGet]
        public ActionResult TopBoxOfice(int page = 1, int? pageSize = 10, string searchString = "")
        {
            List<int> Score = new List<int>();
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = MovieDAO.Instance.TopBox(page, (int)pageSize, searchString);

            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }
        [HttpGet]
        public ActionResult CertifiedMovies(int page = 1, int? pageSize = 10, string searchString = "")
        {
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = MovieDAO.Instance.CertifiedMovies(page, (int)pageSize, searchString);

            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }
        [HttpGet]
        public ActionResult AllMovies(int page = 1, int? pageSize = 10, string searchString = "")
        {
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = MovieDAO.Instance.AllMovies(page, (int)pageSize, searchString);

            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }




        [HttpGet]
        public ActionResult TVOpeningThisMonth(int page = 1, int? pageSize = 10, string searchString = "")
        {
            List<int> Score = new List<int>();
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = TVShowDAO.Instance.OpeningThisMonth(page, (int)pageSize, searchString);
            foreach (var item in result)
            {
                Score.Add(MovieDAO.Instance.GetLemonCatScoreByMovieID(item.MaPhim));
            }
            ViewBag.Score = Score;
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }
        [HttpGet]
        public ActionResult ComingSoomToTV(int page = 1, int? pageSize = 10, string searchString = "")
        {
            List<int> Score = new List<int>();
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = TVShowDAO.Instance.ComingSoon(page, (int)pageSize, searchString);
            foreach (var item in result)
            {
                Score.Add(MovieDAO.Instance.GetLemonCatScoreByMovieID(item.MaPhim));
            }
            ViewBag.Score = Score;
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }
        [HttpGet]
        public ActionResult CertifiedFreshTVShow(int page = 1, int? pageSize = 10, string searchString = "")
        {
            List<int> Score = new List<int>();
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = TVShowDAO.Instance.Fresh(page, (int)pageSize, searchString);
            foreach (var item in result)
            {
                Score.Add(MovieDAO.Instance.GetLemonCatScoreByMovieID(item.MaPhim));
            }
            ViewBag.Score = Score;
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }
        [HttpGet]
        public ActionResult AllTVShow(int page = 1, int? pageSize = 10, string searchString = "")
        {
            List<int> Score = new List<int>();
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Movies" },
             new SelectListItem() { Value="10", Text= "10 Movies" },
             new SelectListItem() { Value="20", Text= "20 Movies" },
             new SelectListItem() { Value="25", Text= "25 Movies" },
             new SelectListItem() { Value="50", Text= "50 Movies" },
            };
            ViewBag.Pagesize = pageSize;
            var result = TVShowDAO.Instance.AllTVShow(page, (int)pageSize, searchString);
            foreach (var item in result)
            {
                Score.Add(MovieDAO.Instance.GetLemonCatScoreByMovieID(item.MaPhim));
            }
            ViewBag.Score = Score;
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }

        [HttpGet]
        public ActionResult TopStar(int page = 1, int? pageSize = 10, string searchString = "")
        {
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Person" },
             new SelectListItem() { Value="10", Text= "10 Person" },
             new SelectListItem() { Value="20", Text= "20 Person" },
             new SelectListItem() { Value="25", Text= "25 Person" },
             new SelectListItem() { Value="50", Text= "50 Person" },
            };
            ViewBag.Pagesize = pageSize;
            var result = PersonDAO.Instance.GetAllPersonStar(page, (int)pageSize, searchString);
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }
        [HttpGet]
        public ActionResult AllStar(int page = 1, int? pageSize = 10, string searchString = "")
        {
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Person" },
             new SelectListItem() { Value="10", Text= "10 Person" },
             new SelectListItem() { Value="20", Text= "20 Person" },
             new SelectListItem() { Value="25", Text= "25 Person" },
             new SelectListItem() { Value="50", Text= "50 Person" },
            };
            ViewBag.Pagesize = pageSize;
            var result = PersonDAO.Instance.GetAllPersonNonStar(page, (int)pageSize, searchString);
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            return View();
        }

        [HttpGet]
        public ActionResult HotReview(int page = 1, int? pageSize = 10, string searchString = "")
        {
            //id->ReviewID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Review" },
             new SelectListItem() { Value="10", Text= "10 Review" },
             new SelectListItem() { Value="20", Text= "20 Review" },
             new SelectListItem() { Value="25", Text= "25 Review" },
             new SelectListItem() { Value="50", Text= "50 Review" },
            };
            ViewBag.Pagesize = pageSize;
            var result = FlimDAO.Instance.GetHotReviewClient(page, (int)pageSize, searchString);
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            List<string> Avata = new List<string>();
            List<string> Name = new List<string>();
            foreach (var item in result)
            {
                Avata.Add(MovieDAO.Instance.GetMovieAvataByMovieID((int)item.MaPhim));
                Name.Add(UserDAO.Instance.GetFullNameByID((int)item.MaTK));
            }
            ViewBag.Avata = Avata;
            ViewBag.Name = Name;
            return View();
        }
        [HttpGet]
        public ActionResult AllReview(int page = 1, int? pageSize = 10, string searchString = "")
        {
            //id->ReviewID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Review" },
             new SelectListItem() { Value="10", Text= "10 Review" },
             new SelectListItem() { Value="20", Text= "20 Review" },
             new SelectListItem() { Value="25", Text= "25 Review" },
             new SelectListItem() { Value="50", Text= "50 Review" },
            };
            ViewBag.Pagesize = pageSize;
            var result = FlimDAO.Instance.GetAllReviewClient(page, (int)pageSize, searchString);
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            List<string> Avata = new List<string>();
            List<string> Name = new List<string>();
            foreach (var item in result)
            {
                Avata.Add(MovieDAO.Instance.GetMovieAvataByMovieID((int)item.MaPhim));
                Name.Add(UserDAO.Instance.GetFullNameByID((int)item.MaTK));
            }
            ViewBag.Avata = Avata;
            ViewBag.Name = Name;
            return View();
        }

        [HttpGet]
        public ActionResult HotNews(int page = 1, int? pageSize = 10, string searchString = "")
        {
            //id->ReviewID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Review" },
             new SelectListItem() { Value="10", Text= "10 Review" },
             new SelectListItem() { Value="20", Text= "20 Review" },
             new SelectListItem() { Value="25", Text= "25 Review" },
             new SelectListItem() { Value="50", Text= "50 Review" },
            };
            ViewBag.Pagesize = pageSize;
            var result = NewsDAO.Instance.GetHotNewsClient(page, (int)pageSize, searchString);
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            List<string> Avata = new List<string>();
            foreach (var item in result)
            {
                Avata.Add(NewsDAO.Instance.GetImageToShowByNewsID((int)item.MaNew));
            }
            ViewBag.Avata = Avata;
            return View();
        }
        [HttpGet]
        public ActionResult AllNews(int page = 1, int? pageSize = 10, string searchString = "")
        {
            //id->ReviewID
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Dropdown = new List<SelectListItem>(){
             new SelectListItem() { Value="5", Text= "5 Review" },
             new SelectListItem() { Value="10", Text= "10 Review" },
             new SelectListItem() { Value="20", Text= "20 Review" },
             new SelectListItem() { Value="25", Text= "25 Review" },
             new SelectListItem() { Value="50", Text= "50 Review" },
            };
            ViewBag.Pagesize = pageSize;
            var result = NewsDAO.Instance.GetAllNewsClient(page, (int)pageSize, searchString);
            ViewBag.Movie = result;
            ViewBag.SearchString = searchString;
            ViewBag.Count = result.Count();
            List<string> Avata = new List<string>();
            foreach (var item in result)
            {
                Avata.Add(NewsDAO.Instance.GetImageToShowByNewsID((int)item.MaNew));
            }
            ViewBag.Avata = Avata;
            return View();
        }

        public ActionResult Search(string searchString)
        {
            Session["searchString"] = searchString;
            ViewBag.SearchString = searchString;
            ViewBag.ListPanel = MovieDAO.Instance.GetBySearchString(searchString);
            ViewBag.ListPanel1 = TVShowDAO.Instance.GetBySearchString(searchString);
            ViewBag.Movie = PersonDAO.Instance.GetBySearchString(searchString);

            var listReview = FlimDAO.Instance.GetBySearchString(searchString);
            List<string> AvataReview = new List<string>();
            List<string> NameReview = new List<string>();
            foreach (var item in listReview)
            {
                AvataReview.Add(MovieDAO.Instance.GetMovieAvataByMovieID((int)item.MaPhim));
                NameReview.Add(UserDAO.Instance.GetFullNameByID((int)item.MaTK));
            }
            ViewBag.ReviewAvata = AvataReview;
            ViewBag.ReviewName = NameReview;
            ViewBag.Review = listReview;

            var listNews = NewsDAO.Instance.GetBySearchString(searchString);
            List<string> AvataNews = new List<string>();
            foreach (var item in listNews)
            {
                AvataNews.Add(UserDAO.Instance.GetAvataByID((int)item.MaTK));
            }
            ViewBag.NewsAvata = AvataNews;
            ViewBag.News = listNews;
            return View();
        }
    }
}