using LemonCat.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class PersonDAO
    {
        private static PersonDAO instance;
        LemonCatEntities db = null;
        public PersonDAO()
        {
            db = new LemonCatEntities();
        }

        public static PersonDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new PersonDAO();
                return PersonDAO.instance;
            }
            private set
            {
                PersonDAO.instance = value;
            }
        }
        public List<DIENVIEN> GetAllPerson()
        {
            return db.DIENVIENs.ToList();
        }
        public bool IsExistPerson(DIENVIEN entity)
        {
            int count = db.DIENVIENs.Count(n=>n.TenDienVien == entity.TenDienVien && n.NgaySinh == entity.NgaySinh && n.NoiSinh == entity.NoiSinh);
            if (count > 0)
                return true;
            return false;
        }
        public int InsertImage(string path, int id, string name, string date, int size)
        {
            ANHDIENVIEN result = new ANHDIENVIEN();
            result.MaDienVien = id;
            result.Anh = path;
            result.TenAnh = name;
            result.NgayCapNhap = date;
            result.KichThuoc = size + "";
            db.ANHDIENVIENs.Add(result);
            db.SaveChanges();
            return result.ID;
        }
        public string GetNameByID(int id)
        {
            return db.DIENVIENs.Find(id).TenDienVien;
        }
        public int Insert(DIENVIEN entity)
        {
            if (!IsExistPerson(entity))
            {
                
                db.DIENVIENs.Add(entity);
                db.SaveChanges();
                return entity.MaDienVien;
            }
            return -1;
        }
        public DIENVIEN GetByID(int id)
        {
            return db.DIENVIENs.Find(id);
        }
        public bool Delete(int id)
        {
            //Delete in AnhDienVien Table
            var listImagePerson = db.ANHDIENVIENs.Where(n => n.MaDienVien == id);
            foreach (var item in listImagePerson)
            {
                db.ANHDIENVIENs.Remove(item);
            }
            db.SaveChanges();

            //Delete in ChiTietDienVienNew Table
            var listNewPerson = db.CHITIETDIENVIENNEWs.Where(n => n.MaDienVien == id);
            foreach (var item in listNewPerson)
            {
                db.CHITIETDIENVIENNEWs.Remove(item);
            }
            db.SaveChanges();

            //Delete in DaoDienPhim Table
            var listDirectedPerson = db.DAODIENPHIMs.Where(n => n.MaDienVien == id);
            foreach (var item in listDirectedPerson)
            {
                db.DAODIENPHIMs.Remove(item);
            }
            db.SaveChanges();

            //Delete in KichBanPhim Table
            var listWrittenPerson = db.KICHBANPHIMs.Where(n => n.MaDienVien == id);
            foreach (var item in listWrittenPerson)
            {
                db.KICHBANPHIMs.Remove(item);
            }
            db.SaveChanges();

            //Delete in DienVienPhim Table
            var listMoviePerson = db.DIENVIENPHIMs.Where(n => n.MaDienVien == id);
            foreach (var item in listMoviePerson)
            {
                db.DIENVIENPHIMs.Remove(item);
            }
            db.SaveChanges();

            //Delete in DienVien Table
            db.DIENVIENs.Remove(GetByID(id));
            db.SaveChanges();
            return true;
        }

        public List<ANHDIENVIEN> GetAllImageByID(int id)
        {
            return db.ANHDIENVIENs.Where(n => n.MaDienVien == id).ToList();
        }
        public ANHDIENVIEN GetImageByID(int id)
        {
            return db.ANHDIENVIENs.Find(id);
        }
        public string GetAvataByID(int id)
        {
            return db.DIENVIENs.Find(id).AnhDaiDien;
        }
        public bool DeleteImage(int id)
        {
            db.ANHDIENVIENs.Remove(GetImageByID(id));
            db.SaveChanges();
            return true;
        }
        public bool UpdateImage(string path, int id, string name, string date, int size)
        {
            ANHDIENVIEN result = db.ANHDIENVIENs.Find(id);
            result.Anh = path;
            result.TenAnh = name;
            result.NgayCapNhap = date;
            result.KichThuoc = size + "";
            db.SaveChanges();
            return true;
        }

        public bool Update(DIENVIEN entity)
        {
            var dienvien = db.DIENVIENs.Find(entity.MaDienVien);
            dienvien.TenDienVien = entity.TenDienVien;
            dienvien.TieuSu = entity.TieuSu;
            dienvien.NoiSinh = entity.NoiSinh;
            dienvien.NgaySinh = entity.NgaySinh;
            if(entity.AnhDaiDien != null)
                dienvien.AnhDaiDien = entity.AnhDaiDien;
            db.SaveChanges();
            return true;
        }

        public string GetNamePersonByImageID(int id)
        {
            int PersonID = (int)db.ANHDIENVIENs.Find(id).MaDienVien;
            return db.DIENVIENs.SingleOrDefault(n => n.MaDienVien == PersonID).TenDienVien;
        }

        public List<ANHDIENVIEN> GetSomeImagePersonByPersonID(int id)
        {
            int count = db.ANHDIENVIENs.Where(n => n.MaDienVien == id).Count();
            if (count > 4)
            {
                count = 4;
            }
            return db.ANHDIENVIENs.Where(n => n.MaDienVien == id).Take(count).ToList();
        }

        public Tuple<List<PHIM>, List<string>, List<string>, int> GetMovieByPersonID(int id)
        {
            var list = db.DIENVIENPHIMs.Where(n => n.MaDienVien == id);
            List<PHIM> result = new List<PHIM>();
            List<string> sresult = new List<string>();
            List<string> yresult = new List<string>();
            foreach (var item in list)
            {
                var temp = MovieDAO.Instance.GetMovie((int)item.MaPhim);
                result.Add(temp);
                if (temp.Theaters != null)
                {
                    if (temp.Theaters.Split('-')[2] != null)
                    {
                        yresult.Add(temp.Theaters.Split('-')[2]);
                    }
                    else
                    {
                        yresult.Add("");
                    }
                }
                else
                {
                    yresult.Add("");
                }
                sresult.Add(item.TenNhanVat);
            }
            int count = list.Count();
            return new Tuple<List<PHIM>, List<string>, List<string>, int>(result, sresult, yresult, count);
        }

        public int CountImage(int id)
        {
            return db.ANHDIENVIENs.Where(n => n.MaDienVien == id).Count();
        }

        public List<TAGS_PERSON> GetTag(int id)
        {
            return db.TAGS_PERSON.Where(n => n.MaDienVien == id).ToList();
        }

        public IEnumerable<DIENVIEN> GetAllPersonStar(int page, int pageSize, string searchString)
        {
            var result = GetAllPerson().Where(n => n.TenDienVien.Contains("") && n.Star == true);
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenDienVien.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.TenDienVien).ToPagedList(page, pageSize);
            return result;
        }
        public IEnumerable<DIENVIEN> GetAllPersonNonStar(int page, int pageSize, string searchString)
        {
            var result = GetAllPerson().Where(n => n.TenDienVien.Contains(""));
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenDienVien.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.TenDienVien).ToPagedList(page, pageSize);
            return result;
        }

        public List<DIENVIEN> GetBySearchString(string searchString)
        {
            return db.DIENVIENs.Where(n => n.TenDienVien.Contains(searchString)).ToList();
        }
    }
}