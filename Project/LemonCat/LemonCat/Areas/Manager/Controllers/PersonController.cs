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
    public class PersonController : BaseController
    {
        // GET: Admin/Person
        public ActionResult Index()
        {
            return View(PersonDAO.Instance.GetAllPerson());
        }
        public ActionResult Create()
        {
            return View();
        }
        public string CreateFloderFlimImage(string flodername)
        {
            string folder = Server.MapPath(string.Format("~/Flim Image/Person/{0}/", flodername));
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return "/Flim Image/Person/" + flodername;
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
        public string ReplaceInvalidChars(string filename)
        {
            return string.Join(" ", filename.Split(Path.GetInvalidFileNameChars()));
        }
        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            int itemp = 0;
            string stemp = "";
            DIENVIEN entity = new DIENVIEN();
            entity.TenDienVien = ReplaceInvalidChars(Request.Form["TenDienVien"]);
            entity.NoiSinh = Request.Form["NoiSinh"];
            entity.TieuSu = Request.Form["TieuSu"];
            entity.NgaySinh = Request.Form["NgaySinh"].Replace("/", "-");
            string basepath = CreateFloderFlimImage(entity.TenDienVien);
            entity.AnhDaiDien = SaveImageMovie(Request.Files["AnhDaiDien"], basepath, ref stemp, ref itemp);
            int id = PersonDAO.Instance.Insert(entity);
            if (id > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 1; i < files.Count; i++)
                {
                    string name = "";
                    int size = 0;
                    string path = SaveImageMovie(files[i], basepath, ref name, ref size);
                    PersonDAO.Instance.InsertImage(path, id, name, DateTime.Now.ToString("dd/MM/yyyy"), size);
                }
            }
            return RedirectToAction("Index", "Person");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(PersonDAO.Instance.GetByID(id));
        }
        [HttpPost]
        public ActionResult Edit(FormCollection form, HttpPostedFileBase AnhDaiDien)
        {
            string stemp = "";
            int itemp = 0;

            DIENVIEN dienvien = new DIENVIEN();
            dienvien.MaDienVien = int.Parse(Request.Form["MaDienVien"]);
            dienvien.TenDienVien = Request.Form["TenDienVien"];
            dienvien.NoiSinh = Request.Form["NoiSinh"];
            dienvien.NgaySinh = Request.Form["NgaySinh"].Replace("/", "-");
            dienvien.TieuSu = Request.Form["TieuSu"];
            string basepath = CreateFloderFlimImage(dienvien.TenDienVien);

            if (AnhDaiDien != null)
                dienvien.AnhDaiDien = SaveImageMovie(AnhDaiDien, basepath, ref stemp, ref itemp);
            else
                dienvien.AnhDaiDien = null;
            PersonDAO.Instance.Update(dienvien);
            return RedirectToAction("Index", "Person");
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool status = PersonDAO.Instance.Delete(id);
            return Json(new
            {
                PersonDeleteStatus = status
            });
        }
        public ActionResult PersonImage(int id)
        {
            ViewBag.PersonName = PersonDAO.Instance.GetNameByID(id);
            ViewBag.IDImagePerson = id;
            return View(PersonDAO.Instance.GetAllImageByID(id));
        }
        public ActionResult EditImagePerson(int id)
        {
            return View(PersonDAO.Instance.GetImageByID(id));
        }

        [HttpPost]
        public JsonResult DeleteImagePerson(int id)
        {
            bool status = PersonDAO.Instance.DeleteImage(id);
            return Json(new
            {
                DeleteImagePerson = status
            });
        }
        [HttpPost]
        public ActionResult EditImagePerson(FormCollection form, HttpPostedFileBase Anh)
        {
            HttpFileCollectionBase file = Request.Files;
            int id = int.Parse(form["ID"]);
            string personname = PersonDAO.Instance.GetNamePersonByImageID(id);
            string name = "";
            int size = 0;
            string basepath = CreateFloderFlimImage(personname);
            if (file[0] != null)
            {
                string path = SaveImageMovie(file[0], basepath, ref name, ref size);
                bool status = PersonDAO.Instance.UpdateImage(path, id, name, DateTime.Now.ToString("dd/MM/yyyy"), size);
            }
            
            return RedirectToAction("Index", "Person");
        }
        public ActionResult AddImagePerson(int id)
        {
            ViewBag.IDPerson = id;
            return View();
        }
        [HttpPost]
        public ActionResult AddImagePerson(FormCollection form)
        {
            HttpFileCollectionBase files = Request.Files;
            int id = int.Parse(form["ID"]);
            string personname = PersonDAO.Instance.GetByID(id).TenDienVien;
            string basepath = CreateFloderFlimImage(personname);
            for (int i = 0; i < files.Count; i++)
            {
                string name = "";
                int size = 0;
                string path = SaveImageMovie(files[i], basepath, ref name, ref size);
                PersonDAO.Instance.InsertImage(path, id, name, DateTime.Now.ToString("dd/MM/yyyy"), size);
            }
            return RedirectToAction("Index", "Person");
        }
    }
}