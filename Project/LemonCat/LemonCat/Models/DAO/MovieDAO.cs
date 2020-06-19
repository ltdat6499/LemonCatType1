using LemonCat.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class MovieDAO
    {
        private static MovieDAO instance;
        LemonCatEntities db = null;
        public MovieDAO()
        {
            db = new LemonCatEntities();
        }

        public static MovieDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new MovieDAO();
                return MovieDAO.instance;
            }
            private set
            {
                MovieDAO.instance = value;
            }
        }
        public PHIM GetMovieByGenreID(int id)
        {
            var list = db.GENREPHIMs.Where(n => n.MaGenre == id);
            foreach (var item in list)
            {
                if (IsExist((int)item.MaPhim) > 0)
                {
                    return GetMovie((int)item.MaPhim);
                }
            }
            return null;
        }
        public int IsExist(int id)
        {
            return db.PHIMs.Where(n => n.MaPhim == id).Count();
        }
        public int CountGenre()
        {
            return db.GENREs.Count();
        }
        public int GetLemonCatScoreByMovieID(int maPhim)
        {
            return (int)db.SCOREs.Where(n => n.MaPhim == maPhim).FirstOrDefault().LemonCatScore;
        }
        public int GetUserScoreByMovieID(int maPhim)
        {
            return (int)db.SCOREs.Where(n => n.MaPhim == maPhim).FirstOrDefault().UserScore;
        }
        public SCORE GetScoreByMovieID(int id)
        {
            return db.SCOREs.Where(n => n.MaPhim == id).FirstOrDefault();
        }

        public List<PHIM> GetAllMovie()
        {
            return db.PHIMs.Where(n=>n.IsTVShow == 0).ToList();
        }
        public PHIM GetMovie(int id)
        {
            return db.PHIMs.SingleOrDefault(n=>n.MaPhim == id);
        }
        public int Insert(PHIM entity)
        {
            entity.IsTVShow = 0;
            entity.BoxOffice = 0;
            entity.IMDBScore = 0;
            entity.LemonCatScore = 0;
            db.PHIMs.Add(entity);
            db.SaveChanges();
            return entity.MaPhim;
        }
        public int InsertImageToMovie(string path, int id, string name, string date, string size)
        {
            ANHPHIM anhphim = new ANHPHIM();
            anhphim.MaPhim = id;
            anhphim.Anh = path;
            anhphim.TenAnh = name;
            anhphim.NgayCapNhap = date;
            anhphim.KichThuoc = size;
            db.ANHPHIMs.Add(anhphim);
            db.SaveChanges();
            return anhphim.ID;
        }

        public void AddDataSet(int id, int userID)
        {
            var result = GetMovie(id);
            List<int> Theloai = new List<int>();
            foreach (var item in result.GENREPHIMs)
            {
                Theloai.Add((int)item.MaGenre);
            }
            
            foreach (var item in Theloai)
            {
                DATASET temp = new DATASET();
                temp.MaTK = userID;
                temp.KetQua = id;
                temp.TheLoai = item;
                db.DATASETs.Add(temp);
                db.SaveChanges();
            }
            
        }

        public List<DIENVIEN> GetCastStarMovieByMovieID(int id)
        {
            var list = db.DIENVIENPHIMs.Where(n => n.MaPhim == id);
            List<DIENVIEN> result = new List<DIENVIEN>();
            foreach (var item in list)
            {
                if (PersonDAO.Instance.GetByID((int)item.MaDienVien).Star == true)
                {
                    result.Add(PersonDAO.Instance.GetByID((int)item.MaDienVien));
                
                }
            }
            return result;
        }

        public int InsertDirectedToMovie(int movieid, int directedid)
        {
            DAODIENPHIM kbp = new DAODIENPHIM();
            kbp.MaPhim = movieid;
            kbp.MaDienVien = directedid;
            db.DAODIENPHIMs.Add(kbp);
            db.SaveChanges();
            return kbp.ID;
        }
        public int InsertWrittenToMovie(int movieid, int writtenid)
        {
            KICHBANPHIM kbp = new KICHBANPHIM();
            kbp.MaPhim = movieid;
            kbp.MaDienVien = writtenid;
            db.KICHBANPHIMs.Add(kbp);
            db.SaveChanges();
            return kbp.ID;
        }
        public int InsertCastToMovie(int movieid, int castid, string charname)
        {
            DIENVIENPHIM dienvien = new DIENVIENPHIM();
            dienvien.MaPhim = movieid;
            dienvien.MaDienVien = castid;
            dienvien.TenNhanVat = charname;
            db.DIENVIENPHIMs.Add(dienvien);
            db.SaveChanges();
            return dienvien.ID;
        }
        private bool IsExistInRelative(List<PHIM> IDList, int? id)
        {
            foreach (var item in IDList)
            {
                if (item.MaPhim == id)
                    return true;
            }
            return false;
        }
        public IEnumerable<PHIM> FindRelativeMovie(int id, int page, int pagesize)
        {
            List<PHIM> IDList = new List<PHIM>();
            var taglist = GetTag(id);
            foreach (var item in taglist)
            {
                var templist = db.TAGS_MOVIE.Where(n => n.Tag == item.Tag && n.MaPhim != item.MaPhim);
                foreach (var item2 in templist)
                {
                    if (db.PHIMs.Where(n => n.MaPhim == (int)item2.MaPhim).Count() > 0 && !IsExistInRelative(IDList, item2.MaPhim))
                        IDList.Add(GetMovie((int)item2.MaPhim));
                }
            }
            return IDList.OrderByDescending(n=>n.MaPhim).ToPagedList(page, pagesize);
        }

        public List<TAGS_MOVIE> GetTag(int id)
        {
            return db.TAGS_MOVIE.Where(n => n.MaPhim == id).ToList();
        }

        public List<GENRE> GetAllGenre()
        {
            return db.GENREs.ToList();
        }
        public List<RATING> GetAllRating()
        {
            return db.RATINGs.ToList();
        }
        public string GetNameRatingByMovieID(int id)
        {
            int resultID = (int)GetMovie(id).Rating;
            return db.RATINGs.Find(resultID).TenRating;
        }
        public List<STUDIO> GetAllStudio()
        {
            return db.Studios.ToList();
        }
        public List<DIENVIEN> GetAllPerson()
        {
            return db.DIENVIENs.ToList();
        }
        public bool InsertGenreToMovie(int movieID, int genreID)
        {
            GENREPHIM result = new GENREPHIM();
            result.MaPhim = movieID;
            result.MaGenre = genreID;
            db.GENREPHIMs.Add(result);
            db.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            //Delete in AnhPhim Table
            var listImageMovie = db.ANHPHIMs.Where(n => n.MaPhim == id);
            foreach (var item in listImageMovie)
            {
                db.ANHPHIMs.Remove(item);
            }
            db.SaveChanges();

            //Delete in GENREPHIMs Table
            var listGenreMovie = db.GENREPHIMs.Where(n => n.MaPhim == id);
            foreach (var item in listGenreMovie)
            {
                db.GENREPHIMs.Remove(item);
            }
            db.SaveChanges();

            //Delete in DAODIENPHIMs Table
            var listDirectorMovie = db.DAODIENPHIMs.Where(n => n.MaPhim == id);
            foreach (var item in listDirectorMovie)
            {
                db.DAODIENPHIMs.Remove(item);
            }
            db.SaveChanges();

            //Delete in KICHBANPHIMs Table
            var listWrittenMovie = db.KICHBANPHIMs.Where(n => n.MaPhim == id);
            foreach (var item in listWrittenMovie)
            {
                db.KICHBANPHIMs.Remove(item);
            }
            db.SaveChanges();

            //Delete in DIENVIENPHIMs Table
            var listCastMovie = db.DIENVIENPHIMs.Where(n => n.MaPhim == id);
            foreach (var item in listCastMovie)
            {
                db.DIENVIENPHIMs.Remove(item);
            }
            db.SaveChanges();

            //Delete in Review Table

            //Delete in Score Table
            var score = db.SCOREs.SingleOrDefault(n => n.MaPhim == id);
            db.SCOREs.Remove(score);

            //Delete in Phim Table
            db.PHIMs.Remove(GetMovie(id));
            db.SaveChanges();
            return true;
        }
        public string GetMovieNameByImageMovieID(int id)
        {
            int movieID = (int)db.ANHPHIMs.Find(id).MaPhim;
            return db.PHIMs.SingleOrDefault(n => n.MaPhim == movieID).TenPhim;
        }
        public List<ANHPHIM> GetAllImageMovieByMovieID(int id)
        {
            return db.ANHPHIMs.Where(n => n.MaPhim == id).ToList();
        }
        public List<ANHPHIM> GetSomeImageMovieByMovieID(int id)
        {
            int count = db.ANHPHIMs.Where(n => n.MaPhim == id).Count();
            if (count > 4)
            {
                count = 4;
            }
            return db.ANHPHIMs.Where(n => n.MaPhim == id).Take(count).ToList();
        }
        public ANHPHIM GetImageMovie(int id)
        {
            return db.ANHPHIMs.SingleOrDefault(n => n.ID == id);
        }
        public bool DeleteImage(int id)
        {
            db.ANHPHIMs.Remove(GetImageMovie(id));
            db.SaveChanges();
            return true;
        }

        public bool UpdateImageMovie(string path, int id, string name, string date, int size)
        {
            ANHPHIM result = db.ANHPHIMs.Find(id);
            result.Anh = path;
            result.TenAnh = name;
            result.NgayCapNhap = date;
            result.KichThuoc = size + "";
            db.SaveChanges();
            return true;
        }

        public int InsertImage(string path, int id, string name, string date, int size)
        {
            ANHPHIM result = new ANHPHIM();
            result.MaPhim = id;
            result.Anh = path;
            result.TenAnh = name;
            result.NgayCapNhap = date;
            result.KichThuoc = size + "";
            db.ANHPHIMs.Add(result);
            db.SaveChanges();
            return result.ID;
        }
        public string GetMovieAvataByMovieID(int id)
        {
            return db.PHIMs.Find(id).AnhDaiDien;
        }
        public string GetMovieNameByMovieID(int id)
        {
            return db.PHIMs.Find(id).TenPhim;
        }

        public List<ANHPHIM> GetImageByMovieID(int id)
        {
            return db.ANHPHIMs.Where(n => n.MaPhim == id).ToList();
        }
        public int CountImageByMovieID(int id)
        {
            return db.ANHPHIMs.Where(n => n.MaPhim == id).Count();
        }

        public ANHPHIM GetImageMovieByImageID(int id)
        {
            return db.ANHPHIMs.Find(id);
        }
        public List<GENREPHIM> GetGenreMovieByMovieID(int id)
        {
            return db.GENREPHIMs.Where(n => n.MaPhim == id).ToList();
        }
        public string GetGenreByGenreID(int id)
        {
            return db.GENREs.Find(id).TenGenre;
        }
        public List<string> GetGenreNameMovieByMovieID(int id)
        {
            var list = GetGenreMovieByMovieID(id);
            List<string> result = new List<string>();
            foreach (var item in list)
            {
                result.Add(db.GENREs.Find(item.MaGenre).TenGenre);
            }
            return result;
        }
        public List<KICHBANPHIM> GetWrittenMovieByMovieID(int id)
        {
            return db.KICHBANPHIMs.Where(n => n.MaPhim == id).ToList();
        }
        public List<DAODIENPHIM> GetDirectorMovieByMovieID(int id)
        {
            return db.DAODIENPHIMs.Where(n => n.MaPhim == id).ToList();
        }

        public int Update(PHIM entity)
        {
            PHIM result = GetMovie(entity.MaPhim);
            result.TenPhim = entity.TenPhim;
            if (entity.AnhDaiDien != null)
                result.AnhDaiDien = entity.AnhDaiDien;
            result.TomTat = entity.TomTat;
            result.TomTatNgan = entity.TomTatNgan;
            result.Trailer = entity.Trailer;
            result.Theaters = entity.Theaters;
            result.OnDVD = entity.OnDVD;
            result.Time = entity.Time;
            result.Studio = entity.Studio;
            result.Rating = entity.Rating;
            db.SaveChanges();
            return result.MaPhim;
        }
        public void RemoveAllGenreMovieByMovieID(int movieID)
        {
            var listResult = GetGenreMovieByMovieID(movieID);
            foreach (var item in listResult)
            {
                db.GENREPHIMs.Remove(item);
            }
        }

        public void RemoveAllDirectorMovieByMovieID(int movieID)
        {
            var listResult = GetDirectorMovieByMovieID(movieID);
            foreach (var item in listResult)
            {
                db.DAODIENPHIMs.Remove(item);
            }
        }

        public void RemoveAllWrittenMovieByMovieID(int movieID)
        {
            var listResult = GetWrittenMovieByMovieID(movieID);
            foreach (var item in listResult)
            {
                db.KICHBANPHIMs.Remove(item);
            }
        }
        public List<DIENVIENPHIM> GetCastMovieByMovieID(int id)
        {
            List<DIENVIENPHIM> result = db.DIENVIENPHIMs.Where(n => n.MaPhim == id).ToList();
            return result;
        }
        public List<DIENVIENPHIM> GetSomeCastMovieByMovieID(int id)
        {
            int Count = db.DIENVIENPHIMs.Where(n => n.MaPhim == id).Count();
            if (Count > 8)
            {
                Count = 5;
            }
            else if (Count > 5)
            {
                Count = 4;
            }
            else if (Count > 3)
            {
                Count = 2;
            }
            List<DIENVIENPHIM> result = db.DIENVIENPHIMs.Where(n => n.MaPhim == id).Take(Count).ToList();
            return result;
        }
        public int GetCastMovieIDByCastID(int id)
        {
            return db.DIENVIENPHIMs.SingleOrDefault(n => n.ID == id).ID;
        }
        public bool DeleteCast(int id)
        {
            db.DIENVIENPHIMs.Remove(db.DIENVIENPHIMs.Find(id));
            db.SaveChanges();
            return true;
        }

        public string GetCharacterNameByCastID(int id)
        {
            return db.DIENVIENPHIMs.SingleOrDefault(n => n.ID == id).TenNhanVat;
        }

        public void EditCastNameByID(int id, string name)
        {
            var result = db.DIENVIENPHIMs.Find(id);
            result.TenNhanVat = name;
            db.SaveChanges();
        }

        public IEnumerable<PHIM> OpeningThisMonth(int page, int pageSize, string searchString)
        {
            string now = DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-");
            string month = now.Split('-')[1];
            string year = now.Split('-')[2];
            var result = GetAllMovie().Where(n=> n.Theaters.Split('-')[1] == month && n.Theaters.Split('-')[2] == year);
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenPhim.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.TenPhim).ToPagedList(page, pageSize);
            return result;
        }
        public IEnumerable<PHIM> ComingSoon(int page, int pageSize, string searchString)
        {
            string now = DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-");
            string month = now.Split('-')[1];
            string year = now.Split('-')[2];
            var result = GetAllMovie().Where(n => int.Parse(n.Theaters.Split('-')[1]) >= int.Parse(month) && int.Parse(n.Theaters.Split('-')[2]) >= int.Parse(year));
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenPhim.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.TenPhim).ToPagedList(page, pageSize);
            return result;
        }
        public IEnumerable<PHIM> TopBox(int page, int pageSize, string searchString)
        {
            var result = GetAllMovie().Where(n=>n.TenPhim.Contains(""));
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenPhim.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.BoxOffice).ToPagedList(page, pageSize);
            return result;
        }

        public IEnumerable<PHIM> CertifiedMovies(int page, int pageSize, string searchString)
        {
            var result = GetAllMovie().Where(n => n.TenPhim.Contains("") && n.LemonCatScore > 65);
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenPhim.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.TenPhim).ToPagedList(page, pageSize);
            return result;
        }
        public IEnumerable<PHIM> AllMovies(int page, int pageSize, string searchString)
        {
            var result = GetAllMovie().Where(n => n.TenPhim.Contains(""));
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenPhim.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.TenPhim).ToPagedList(page, pageSize);
            return result;
        }

        public List<PHIM> GetBySearchString(string searchString)
        {
            return db.PHIMs.Where(n => n.IsTVShow == 0 && n.TenPhim.Contains(searchString)).ToList();
        }
    }
}