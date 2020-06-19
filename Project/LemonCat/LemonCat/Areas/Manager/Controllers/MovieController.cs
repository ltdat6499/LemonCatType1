using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LemonCat.Models.DAO;
using LemonCat.Models.EF;

namespace LemonCat.Areas.Manager.Controllers
{
    public class MovieController : BaseController
    {
        // GET: Admin/Movie
        public ActionResult Index()
        {
            var list = MovieDAO.Instance.GetAllMovie();
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
            string folder = Server.MapPath(string.Format("~/Flim Image/Movie/{0}/", flodername));
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return "/Flim Image/Movie/" + flodername;
        }
        public string ReplaceInvalidChars(string filename)
        {
            return string.Join(" ", filename.Split(Path.GetInvalidFileNameChars()));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.PersonList = MovieDAO.Instance.GetAllPerson();
            ViewBag.StudioList = MovieDAO.Instance.GetAllStudio();
            ViewBag.RatingList = MovieDAO.Instance.GetAllRating();
            ViewBag.GenreList = MovieDAO.Instance.GetAllGenre();
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
            phim.Theaters = form["Theaters"].Replace("/", "-");
            phim.OnDVD = form["OnDVD"].Replace("/", "-");
            phim.Time = int.Parse(form["Time"]);
            phim.Studio = int.Parse(form["Studio"]);
            phim.Rating = int.Parse(form["Rating"]);
            int movieID = MovieDAO.Instance.Insert(phim);

            if (form["Genre"] != null && movieID > 0)
            {
                string[] Genre = form["Genre"].Split(',');
                foreach (string item in Genre)
                {
                    MovieDAO.Instance.InsertGenreToMovie(movieID, int.Parse(item));
                }
            }
            if (form["DaoDien"] != null && movieID > 0)
            {
                string[] DaoDien = form["DaoDien"].Split(',');
                foreach (string item in DaoDien)
                {
                    MovieDAO.Instance.InsertDirectedToMovie(movieID, int.Parse(item));
                }
            }
            if (form["KichBan"] != null && movieID > 0)
            {
                string[] KichBan = form["KichBan"].Split(',');
                foreach (string item in KichBan)
                {
                    MovieDAO.Instance.InsertWrittenToMovie(movieID, int.Parse(item));
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
                    MovieDAO.Instance.InsertImageToMovie(path, movieID, name, DateTime.Now.ToString("dd/MM/yyyy"), size + "");
                }
            }
            if (form["Cast"] != null)
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
                    MovieDAO.Instance.InsertCastToMovie(movieID, PersonIDList[i], characterName[i]);
                }
            }

            return RedirectToAction("Index", "Movie");
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
        public JsonResult Delete(int id)
        {
            // id->movieid
            bool status = MovieDAO.Instance.Delete(id);
            return Json(new
            {
                MovieDeleteStatus = status
            });
        }
        public ActionResult MovieImage(int id)
        {
            // id->movieid
            ViewBag.MovieName = MovieDAO.Instance.GetMovieNameByMovieID(id);
            ViewBag.IDImageMovie = id;
            return View(MovieDAO.Instance.GetAllImageMovieByMovieID(id));
        }
        [HttpPost]
        public JsonResult DeleteImageMovie(int id)
        {
            // id->imageid
            bool status = MovieDAO.Instance.DeleteImage(id);
            return Json(new
            {
                DeleteImageMovie = status
            });
        }
        public ActionResult EditImageMovie(int id)
        {
            // id->imageid
            ViewBag.MovieName = MovieDAO.Instance.GetMovieNameByImageMovieID(id);
            return View(MovieDAO.Instance.GetImageMovieByImageID(id));
        }
        [HttpPost]
        public ActionResult EditImageMovie(FormCollection form, HttpPostedFileBase Anh)
        {
            // id->imageid
            HttpFileCollectionBase file = Request.Files;
            int id = int.Parse(form["ID"]);
            string moviename = MovieDAO.Instance.GetMovieNameByImageMovieID(id);
            string name = "";
            int size = 0;
            string basepath = CreateFloderFlimImage(moviename);
            if (file[0] != null)
            {
                string path = SaveImageMovie(file[0], basepath, ref name, ref size);
                bool status = MovieDAO.Instance.UpdateImageMovie(path, id, name, DateTime.Now.ToString("dd/MM/yyyy"), size);
            }

            return RedirectToAction("Index", "Movie");
        }
        [HttpGet]
        public ActionResult AddImageMovie(int id)
        {
            // id->movieid
            ViewBag.IDMovie = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddImageMovie(FormCollection form)
        {
            // id->movieid
            HttpFileCollectionBase files = Request.Files;
            int id = int.Parse(form["ID"]);
            string moviename = MovieDAO.Instance.GetMovieNameByMovieID(id);
            string basepath = CreateFloderFlimImage(moviename);
            for (int i = 0; i < files.Count; i++)
            {
                string name = "";
                int size = 0;
                string path = SaveImageMovie(files[i], basepath, ref name, ref size);
                MovieDAO.Instance.InsertImage(path, id, name, DateTime.Now.ToString("dd/MM/yyyy"), size);
            }
            return RedirectToAction("Index", "Movie");
        }
        public ActionResult EditInfo(int id)
        {
            // id->movieid
            ViewBag.PersonList = MovieDAO.Instance.GetAllPerson();
            ViewBag.StudioList = MovieDAO.Instance.GetAllStudio();
            ViewBag.RatingList = MovieDAO.Instance.GetAllRating();
            ViewBag.GenreList = MovieDAO.Instance.GetAllGenre();

            var tempGenre = MovieDAO.Instance.GetGenreMovieByMovieID(id);
            List<int> Genre = new List<int>();
            foreach (var item in tempGenre)
            {
                Genre.Add((int)item.MaGenre);
            }
            ViewBag.Genre = Genre;

            var tempDirector = MovieDAO.Instance.GetDirectorMovieByMovieID(id);
            List<int> Director = new List<int>();
            foreach (var item in tempDirector)
            {
                Director.Add((int)item.MaDienVien);
            }
            ViewBag.Director = Director;

            var tempWritten = MovieDAO.Instance.GetWrittenMovieByMovieID(id);
            List<int> Written = new List<int>();
            foreach (var item in tempWritten)
            {
                Written.Add((int)item.MaDienVien);
            }
            ViewBag.Written = Written;

            return View(MovieDAO.Instance.GetMovie(id));
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
            phim.Theaters = form["Theaters"].Replace("/", "-");
            phim.OnDVD = form["OnDVD"].Replace("/", "-");
            phim.Time = int.Parse(form["Time"]);
            phim.Studio = int.Parse(form["Studio"]);
            phim.Rating = int.Parse(form["Rating"]);
            int movieID = MovieDAO.Instance.Update(phim);

            if (form["Genre"] != null && movieID > 0)
            {
                string[] Genre = form["Genre"].Split(',');
                MovieDAO.Instance.RemoveAllGenreMovieByMovieID(movieID);
                foreach (string item in Genre)
                {
                    MovieDAO.Instance.InsertGenreToMovie(movieID, int.Parse(item));
                }
            }
            if (form["DaoDien"] != null && movieID > 0)
            {
                string[] DaoDien = form["DaoDien"].Split(',');
                MovieDAO.Instance.RemoveAllDirectorMovieByMovieID(movieID);
                foreach (string item in DaoDien)
                {
                    MovieDAO.Instance.InsertDirectedToMovie(movieID, int.Parse(item));
                }
            }
            if (form["KichBan"] != null && movieID > 0)
            {
                string[] KichBan = form["KichBan"].Split(',');
                MovieDAO.Instance.RemoveAllWrittenMovieByMovieID(movieID);
                foreach (string item in KichBan)
                {
                    MovieDAO.Instance.InsertWrittenToMovie(movieID, int.Parse(item));
                }
            }

            return RedirectToAction("Index", "Movie");
        }
        [HttpPost]
        public JsonResult DeleteCast(int id)
        {
            // id->CastID
            bool status = MovieDAO.Instance.DeleteCast(id);
            return Json(new
            {
                CastDeleteStatus = status
            });
        }
        public ActionResult EditCast(int id)
        {
            // id->movieid
            ViewBag.MovieID = id;
            ViewBag.MovieName = MovieDAO.Instance.GetMovieNameByMovieID(id);
            var listCast = MovieDAO.Instance.GetCastMovieByMovieID(id);
            List<string> ListName = new List<string>();
            List<string> ListNameActor = new List<string>();
            List<string> ListAvata = new List<string>();
            List<int> ListMovieCastID = new List<int>();
            foreach (var item in listCast)
            {
                ListName.Add(MovieDAO.Instance.GetCharacterNameByCastID((int)item.ID));
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
            MovieDAO.Instance.EditCastNameByID(id, name);
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
            ViewBag.PersonList = MovieDAO.Instance.GetAllPerson();
            ViewBag.MovieName = MovieDAO.Instance.GetMovieNameByMovieID(id);
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
                    MovieDAO.Instance.InsertCastToMovie(movieID, PersonIDList[i], characterName[i]);
                }
            }
            return RedirectToAction("Index", "Movie");
        }
    }
}