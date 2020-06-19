using LemonCat.Models.DAO;
using LemonCat.Models.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LemonCat.Areas.Manager.Controllers
{
    public class TVShowController : BaseController
    {
        // GET: Admin/TVShow
        public ActionResult Index()
        {
            var list = TVShowDAO.Instance.GetAllTVShow();
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
        public string CreateFloderFlimImage(string flodername)
        {
            string folder = Server.MapPath(string.Format("~/Flim Image/TV/{0}/", flodername));
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return "/Flim Image/TV/" + flodername;
        }
        public string ReplaceInvalidChars(string filename)
        {
            return string.Join(" ", filename.Split(Path.GetInvalidFileNameChars()));
        }
        public string SaveImageMovie(HttpPostedFileBase image, string path, ref string name, ref int size)
        {
            string basepath = path;
            if (image != null)
            {
                // Get file name
                var filename = Path.GetFileName(image.FileName);
                // Path
                path = Path.Combine(Server.MapPath("~" + basepath), filename);
                // Check Exist file
                if (System.IO.File.Exists(path))
                {
                    // If File Existed -> Add "_" before filename and save again until File not Exits 
                    while (System.IO.File.Exists(path))
                    {
                        filename = "_" + filename;
                        path = Path.Combine(Server.MapPath("~" + basepath), filename);
                    }
                }
                image.SaveAs(path);
                name = filename;
                size = image.ContentLength / 1024;
                return basepath + "/" + filename;
            }
            else
                return null;
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // id->ImageID
            bool status = TVShowDAO.Instance.DeleteTVShowByTVShowID(id);
            return Json(new
            {
                MovieDeleteStatus = status
            });
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.PersonList = TVShowDAO.Instance.GetAllPerson();
            ViewBag.NetworkList = TVShowDAO.Instance.GetAllNetwork();
            ViewBag.RatingList = TVShowDAO.Instance.GetAllRating();
            ViewBag.GenreList = TVShowDAO.Instance.GetAllGenre();
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            string Tempname = "";
            int Tempsize = 0;

            PHIM phim = new PHIM();
            string movieName = ReplaceInvalidChars(form["TenPhim"]);
            phim.TenPhim = movieName;
            string basepath = CreateFloderFlimImage(phim.TenPhim);
            phim.AnhDaiDien = SaveImageMovie(Request.Files[0], basepath, ref Tempname, ref Tempsize);
            phim.TomTatNgan = form["TomTatNgan"];
            phim.TomTat = form["TomTat"];
            phim.Trailer = form["Trailer"];
            phim.OnDVD = form["OnDVD"].Replace("/", "-");
            phim.Time = int.Parse(form["Time"]);            
            phim.Rating = int.Parse(form["Rating"]);
            int movieID = TVShowDAO.Instance.InsertTVShow(phim);

            if (form["Genre"] != null && movieID > 0)
            {
                string[] Genre = form["Genre"].Split(',');
                foreach (string item in Genre)
                {
                    TVShowDAO.Instance.InsertGenreTVShow(movieID, int.Parse(item));
                }
            }
            if (form["DaoDien"] != null && movieID > 0)
            {
                string[] DaoDien = form["DaoDien"].Split(',');
                foreach (string item in DaoDien)
                {
                    TVShowDAO.Instance.InsertDirectedToTVShow(movieID, int.Parse(item));
                }
            }
            if (form["KichBan"] != null && movieID > 0)
            {
                string[] KichBan = form["KichBan"].Split(',');
                foreach (string item in KichBan)
                {
                    TVShowDAO.Instance.InsertWrittenToTVShow(movieID, int.Parse(item));
                }
            }

            if (movieID > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i < files.Count; i++)
                {
                    string name = "";
                    int size = 0;
                    string path = SaveImageMovie(files[i], basepath, ref name, ref size);
                    TVShowDAO.Instance.InsertImageToTVShow(path, movieID, name, DateTime.Now.ToString("dd/MM/yyyy"), size + "");
                }
            }

            if (form["Eps"] != null && movieID > 0)
            {
                List<int> EpsNumber = new List<int>();
                List<string> EpsName = new List<string>();
                string[] spearator = { "*_*_*", "*_*" };
                string[] eachEps = form["Eps"].Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < eachEps.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        EpsNumber.Add(int.Parse(eachEps[i]));
                    }
                    else
                    {
                        EpsName.Add(eachEps[i]);
                    }
                }
                for (int i = 0; i < EpsName.Count; i++)
                {
                    TVShowDAO.Instance.InsertEps(movieID, EpsNumber[i], EpsName[i]);
                }
            }
            if (form["Cast"] != null && movieID > 0)
            {
                //form["Cast"] = form["Cast"].Replace("&nbsp", "");
                //form["Cast"].Trim();
                List<int> PersonIDList = new List<int>();
                List<string> characterName = new List<string>();
                string[] spearator = { "*_*_*", "*_*" };
                string[] eachPerson = form["Cast"].Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < eachPerson.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        PersonIDList.Add(int.Parse(eachPerson[i]));
                    }
                    else
                    {
                        characterName.Add(eachPerson[i]);
                    }
                }
                for (int i = 0; i < PersonIDList.Count; i++)
                {
                    TVShowDAO.Instance.InsertCastToTVShow(movieID, PersonIDList[i], characterName[i]);
                }
            }
            if (movieID > 0)
            {
                int Network = int.Parse(form["Network"]);
                string PremiereDate = form["PremiereDate"].Replace("/", "-");
                int sotap = TVShowDAO.Instance.GetCountEpsByTVShowID(movieID);
                TVShowDAO.Instance.InsertNewTVShowInfo(movieID, Network, PremiereDate, sotap);
            }

            return RedirectToAction("Index", "TVShow");
        }
        [HttpGet]
        public ActionResult MovieImage(int id)
        {
            // id->MovieID
            ViewBag.MovieName = TVShowDAO.Instance.GetTVShowNameByTVShowID(id);
            ViewBag.IDImageMovie = id;
            return View(TVShowDAO.Instance.GetAllImageMovieByMovieID(id));
        }
        [HttpGet]
        public ActionResult AddImageMovie(int id)
        {
            // id->MovieID
            ViewBag.IDMovie = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddImageMovie(FormCollection form)
        {
            // id->MovieID
            HttpFileCollectionBase files = Request.Files;
            int id = int.Parse(form["ID"]);
            string moviename = TVShowDAO.Instance.GetTVShowNameByTVShowID(id);
            string basepath = CreateFloderFlimImage(moviename);
            for (int i = 0; i < files.Count; i++)
            {
                string name = "";
                int size = 0;
                string path = SaveImageMovie(files[i], basepath, ref name, ref size);
                TVShowDAO.Instance.InsertImage(path, id, name, DateTime.Now.ToString("dd/MM/yyyy"), size);
            }
            return RedirectToAction("Index", "TVShow");
        }
        [HttpPost]
        public JsonResult DeleteImageMovie(int id)
        {
            // id->ImageID
            bool status = TVShowDAO.Instance.DeleteImageByImageID(id);
            return Json(new
            {
                MovieDeleteStatus = status
            });
        }
        [HttpGet]
        public ActionResult EditImageMovie(int id)
        {
            // id->ImageID
            ViewBag.TVShowName = TVShowDAO.Instance.GetTVShowNameByImageID(id);
            return View(TVShowDAO.Instance.GetImageTVShowByImageID(id));
        }
        [HttpPost]
        public ActionResult EditImageMovie(FormCollection form, HttpPostedFileBase Anh)
        {
            // id->ImageID
            HttpFileCollectionBase file = Request.Files;
            int id = int.Parse(form["ID"]);
            string moviename = TVShowDAO.Instance.GetTVShowNameByImageID(id);
            string name = "";
            int size = 0;
            string basepath = CreateFloderFlimImage(moviename);
            if (file[0] != null)
            {
                string path = SaveImageMovie(file[0], basepath, ref name, ref size);
                bool status = TVShowDAO.Instance.UpdateImageTVShow(path, id, name, DateTime.Now.ToString("dd/MM/yyyy"), size);
            }

            return RedirectToAction("Index", "TVShow");
        }
        [HttpPost]
        public JsonResult DeleteCast(int id)
        {
            // id->CastID
            bool status = TVShowDAO.Instance.DeleteCast(id);
            return Json(new
            {
                CastDeleteStatus = status
            });
        }
        [HttpGet]
        public ActionResult EditCast(int id)
        {
            //id->movieID
            // id->movieid
            ViewBag.MovieID = id;
            ViewBag.MovieName = TVShowDAO.Instance.GetTVShowNameByTVShowID(id);
            var listCast = TVShowDAO.Instance.GetCastTVShowByTVShowID(id);
            List<string> ListName = new List<string>();
            List<string> ListNameActor = new List<string>();
            List<string> ListAvata = new List<string>();
            List<int> ListMovieCastID = new List<int>();
            foreach (var item in listCast)
            {
                ListName.Add(TVShowDAO.Instance.GetCharacterNameByCastID((int)item.ID));
                ListMovieCastID.Add((int)item.ID);
                ListNameActor.Add(PersonDAO.Instance.GetNameByID((int)item.MaDienVien));
                ListAvata.Add(PersonDAO.Instance.GetAvataByID((int)item.MaDienVien));
            }
            ViewBag.PersonID = ListMovieCastID;
            ViewBag.ListName = ListName;
            ViewBag.ListNameActor = ListNameActor;
            ViewBag.ListAvata = ListAvata;
            return View(listCast);
        }
        [HttpPost]
        public JsonResult EditCastName(int id, string name)
        {
            // id->CastID
            TVShowDAO.Instance.EditCastNameByID(id, name);
            return Json(new
            {
                CastEditNameStatus = true
            }); ;
        }
        [HttpGet]
        public ActionResult CreateNewCast(int id)
        {
            // id->MovieID
            ViewBag.MovieID = id;
            ViewBag.PersonList = TVShowDAO.Instance.GetAllPerson();
            ViewBag.MovieName = TVShowDAO.Instance.GetTVShowNameByTVShowID(id);
            return View();
        }
        [HttpPost]
        public ActionResult CreateNewCast(FormCollection form)
        {
            // id->MovieID
            int movieID = int.Parse(form["MovieID"]);
            if (form["Cast"] != null && movieID > 0)
            {
                //form["Cast"] = form["Cast"].Replace("&nbsp", "");
                //form["Cast"].Trim();
                List<int> PersonIDList = new List<int>();
                List<string> characterName = new List<string>();
                string[] spearator = { "*_*_*", "*_*" };
                string[] eachPerson = form["Cast"].Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < eachPerson.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        PersonIDList.Add(int.Parse(eachPerson[i]));
                    }
                    else
                    {
                        characterName.Add(eachPerson[i]);
                    }
                }
                for (int i = 0; i < PersonIDList.Count; i++)
                {
                    TVShowDAO.Instance.InsertCastToTVShow(movieID, PersonIDList[i], characterName[i]);
                }
            }
            return RedirectToAction("Index", "TVShow");
        }
        public ActionResult EditInfo(int id)
        {
            // id->movieid
            ViewBag.PersonList = TVShowDAO.Instance.GetAllPerson();
            ViewBag.StudioList = TVShowDAO.Instance.GetAllStudio();
            ViewBag.RatingList = TVShowDAO.Instance.GetAllRating();
            ViewBag.GenreList = TVShowDAO.Instance.GetAllGenre();
            ViewBag.NetworkList = TVShowDAO.Instance.GetAllNetwork();

            var tempGenre = TVShowDAO.Instance.GetGenreTVShowByTVShowID(id);
            List<int> Genre = new List<int>();
            foreach (var item in tempGenre)
            {
                Genre.Add((int)item.MaGenre);
            }
            ViewBag.Genre = Genre;

            var tempDirector = TVShowDAO.Instance.GetDirectorTVShowByTVShowID(id);
            List<int> Director = new List<int>();
            foreach (var item in tempDirector)
            {
                Director.Add((int)item.MaDienVien);
            }
            ViewBag.Director = Director;

            var tempWritten = TVShowDAO.Instance.GetWrittenTVShowByTVShowID(id);
            List<int> Written = new List<int>();
            foreach (var item in tempWritten)
            {
                Written.Add((int)item.MaDienVien);
            }
            ViewBag.Written = Written;

            string tempPremiereDate = TVShowDAO.Instance.GetPremiereDateByTVShowID(id);
            ViewBag.PremiereDate = tempPremiereDate;

            int tempNetwork = TVShowDAO.Instance.GetTVShowNetworkByTVShowID(id);
            ViewBag.Network = tempNetwork;


            return View(TVShowDAO.Instance.GetTVShowByTVShowID(id));
        }
        [HttpPost]
        public ActionResult EditInfo(FormCollection form, HttpPostedFileBase AnhDaiDien)
        {
            // id->movieid
            string Tempname = "";
            int Tempsize = 0;

            PHIM phim = new PHIM();
            string movieName = ReplaceInvalidChars(form["TenPhim"]);
            phim.MaPhim = int.Parse(form["ID"]);
            phim.TenPhim = movieName;

            if (AnhDaiDien != null)
            {
                string basepath = CreateFloderFlimImage(phim.TenPhim);
                phim.AnhDaiDien = SaveImageMovie(AnhDaiDien, basepath, ref Tempname, ref Tempsize);
            }

            phim.TomTatNgan = form["TomTatNgan"];
            phim.TomTat = form["TomTat"];
            phim.Trailer = form["Trailer"];
            //phim.Theaters = form["Theaters"];
            phim.OnDVD = form["OnDVD"].Replace("/", "-");
            phim.Time = int.Parse(form["Time"]);
            //phim.Studio = int.Parse(form["Studio"]);
            phim.Rating = int.Parse(form["Rating"]);
            int movieID = TVShowDAO.Instance.Update(phim);

            TVSHOW tvshow = new TVSHOW();
            tvshow.MaPhim = phim.MaPhim;
            tvshow.Network = int.Parse(form["Network"]);
            tvshow.PremiereDate = form["PremiereDate"].Replace("/", "-");
            int idTVshow = TVShowDAO.Instance.UpdateInfo(tvshow);

            
            
            

            if (form["Genre"] != null && movieID > 0)
            {
                string[] Genre = form["Genre"].Split(',');
                TVShowDAO.Instance.RemoveAllGenreTVShowByTVShowID(movieID);
                foreach (string item in Genre)
                {
                    TVShowDAO.Instance.InsertGenreTVShow(movieID, int.Parse(item));
                }
            }
            if (form["DaoDien"] != null && movieID > 0)
            {
                string[] DaoDien = form["DaoDien"].Split(',');
                TVShowDAO.Instance.RemoveAllDirectorTVShowByTVShowID(movieID);
                foreach (string item in DaoDien)
                {
                    TVShowDAO.Instance.InsertDirectedToTVShow(movieID, int.Parse(item));
                }
            }
            if (form["KichBan"] != null && movieID > 0)
            {
                string[] KichBan = form["KichBan"].Split(',');
                TVShowDAO.Instance.RemoveAllWrittenTVShowByTVShowID(movieID);
                foreach (string item in KichBan)
                {
                    TVShowDAO.Instance.InsertWrittenToTVShow(movieID, int.Parse(item));
                }
            }

            return RedirectToAction("Index", "TVShow");
        }
        [HttpGet]
        public ActionResult EditEps(int id)
        {
            //id->tvshowID
            ViewBag.MovieID = id;
            ViewBag.MovieName = TVShowDAO.Instance.GetTVShowNameByTVShowID(id);
            return View(TVShowDAO.Instance.GetAllEpsByTVShowID(id));
        }
        [HttpPost]
        public JsonResult DeleteLastEps(int id)
        {
            // id->EpsID
            bool status = TVShowDAO.Instance.DeleteLastEpsByEpsID(id);
            return Json(new
            {
                CastDeleteStatus = status
            });
        }
        [HttpPost]
        public JsonResult EditEpsName(int id, string name)
        {
            // id->EpsID
            bool status = TVShowDAO.Instance.EditEpsNameByEpsID(id, name);
            return Json(new
            {
                CastEditNameStatus = status
            });
        }
        [HttpGet]
        public ActionResult CreateNewEps(int id)
        {
            //id->TVShowID
            ViewBag.ID = id;
            ViewBag.CountEps = TVShowDAO.Instance.GetCountEpsByTVShowID(id);
            return View();
        }
        [HttpPost]
        public ActionResult CreateNewEps(FormCollection form)
        {
            //id->TVShowID
            int movieID = int.Parse(form["ID"]);
            if (form["Eps"] != null && movieID > 0)
            {
                List<int> EpsNumber = new List<int>();
                List<string> EpsName = new List<string>();
                string[] spearator = { "*_*_*", "*_*" };
                string[] eachEps = form["Eps"].Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < eachEps.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        EpsNumber.Add(int.Parse(eachEps[i]));
                    }
                    else
                    {
                        EpsName.Add(eachEps[i]);
                    }
                }
                for (int i = 0; i < EpsName.Count; i++)
                {
                    TVShowDAO.Instance.InsertEps(movieID, EpsNumber[i], EpsName[i]);
                }
            }
            return RedirectToAction("Index", "TVShow");
        }
    }
}