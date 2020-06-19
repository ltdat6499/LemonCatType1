using LemonCat.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class TVShowDAO
    {
        private static TVShowDAO instance;
        LemonCatEntities db = null;
        public TVShowDAO()
        {
            db = new LemonCatEntities();
        }

        public static TVShowDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new TVShowDAO();
                return TVShowDAO.instance;
            }
            private set
            {
                TVShowDAO.instance = value;
            }
        }
        public List<PHIM> GetAllTVShow()
        {
            return db.PHIMs.Where(n => n.IsTVShow == 1).ToList();
        }
        public PHIM GetTVShowByTVShowID(int id)
        {
            return db.PHIMs.SingleOrDefault(n => n.MaPhim == id);
        }
        public int InsertTVShow(PHIM entity)
        {
            entity.IsTVShow = 1;
            db.PHIMs.Add(entity);
            db.SaveChanges();
            return entity.MaPhim;
        }
        public int InsertImageToTVShow(string path, int id, string name, string date, string size)
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
        public int InsertDirectedToTVShow(int movieid, int directedid)
        {
            DAODIENPHIM kbp = new DAODIENPHIM();
            kbp.MaPhim = movieid;
            kbp.MaDienVien = directedid;
            db.DAODIENPHIMs.Add(kbp);
            db.SaveChanges();
            return kbp.ID;
        }
        public int InsertWrittenToTVShow(int movieid, int writtenid)
        {
            KICHBANPHIM kbp = new KICHBANPHIM();
            kbp.MaPhim = movieid;
            kbp.MaDienVien = writtenid;
            db.KICHBANPHIMs.Add(kbp);
            db.SaveChanges();
            return kbp.ID;
        }

        public List<NETWORK> GetAllNetwork()
        {
            return db.NETWORKs.OrderBy(n => n.NetworkName).ToList();
        }

        public int InsertCastToTVShow(int movieid, int castid, string charname)
        {
            DIENVIENPHIM dienvien = new DIENVIENPHIM();
            dienvien.MaPhim = movieid;
            dienvien.MaDienVien = castid;
            dienvien.TenNhanVat = charname;
            db.DIENVIENPHIMs.Add(dienvien);
            db.SaveChanges();
            return dienvien.ID;
        }
        public List<GENRE> GetAllGenre()
        {
            return db.GENREs.ToList();
        }
        public List<RATING> GetAllRating()
        {
            return db.RATINGs.ToList();
        }
        public List<STUDIO> GetAllStudio()
        {
            return db.Studios.ToList();
        }
        public List<DIENVIEN> GetAllPerson()
        {
            return db.DIENVIENs.ToList();
        }
        public bool InsertGenreTVShow(int movieID, int genreID)
        {
            GENREPHIM result = new GENREPHIM();
            result.MaPhim = movieID;
            result.MaGenre = genreID;
            db.GENREPHIMs.Add(result);
            db.SaveChanges();
            return true;
        }

        public bool DeleteTVShowByTVShowID(int id)
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

            //Delete in Eps
            var listEpsMovie = db.TVSHOWEPs.Where(n => n.MaPhim == id);
            foreach (var item in listEpsMovie)
            {
                db.TVSHOWEPs.Remove(item);
            }
            db.SaveChanges();

            //Delete in TVShowInfo
            var infolist = db.TVSHOWs.Where(n => n.MaPhim == id);
            foreach (var item in infolist)
            {
                db.TVSHOWs.Remove(item);
            }
            db.SaveChanges();

            //Delete in Review Table

            //Delete in Score Table
            var score = db.SCOREs.SingleOrDefault(n => n.MaPhim == id);
            db.SCOREs.Remove(score);
            //Delete in DVD

            //Delete in Phim Table
            db.PHIMs.Remove(GetTVShowByTVShowID(id));
            db.SaveChanges();
            return true;
        }
        public string GetTVShowNameByTVShowID(int id)
        {
            return db.PHIMs.SingleOrDefault(n => n.MaPhim == id).TenPhim;
        }
        public List<ANHPHIM> GetAllImageMovieByMovieID(int id)
        {
            return db.ANHPHIMs.Where(n => n.MaPhim == id).ToList();
        }
        public ANHPHIM GetImageTVShow(int id)
        {
            return db.ANHPHIMs.SingleOrDefault(n => n.ID == id);
        }

        public void InsertEps(int movieID, int EpsNumber, string EpsName)
        {
            TVSHOWEP result = new TVSHOWEP();
            result.MaPhim = movieID;
            result.TenTap = EpsName;
            result.Ep = EpsNumber;
            db.TVSHOWEPs.Add(result);
            db.SaveChanges();
        }

        public bool DeleteImageByImageID(int id)
        {
            db.ANHPHIMs.Remove(GetImageTVShow(id));
            db.SaveChanges();
            return true;
        }

        public bool UpdateImageTVShow(string path, int id, string name, string date, int size)
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

        public ANHPHIM GetImageTVShowByImageID(int id)
        {
            return db.ANHPHIMs.Find(id);
        }
        public List<GENREPHIM> GetGenreTVShowByTVShowID(int id)
        {
            return db.GENREPHIMs.Where(n => n.MaPhim == id).ToList();
        }
        public List<KICHBANPHIM> GetWrittenTVShowByTVShowID(int id)
        {
            return db.KICHBANPHIMs.Where(n => n.MaPhim == id).ToList();
        }
        public List<DAODIENPHIM> GetDirectorTVShowByTVShowID(int id)
        {
            return db.DAODIENPHIMs.Where(n => n.MaPhim == id).ToList();
        }

        public int Update(PHIM entity)
        {
            PHIM result = GetTVShowByTVShowID(entity.MaPhim);
            result.TenPhim = entity.TenPhim;
            if (entity.AnhDaiDien != null)
                result.AnhDaiDien = entity.AnhDaiDien;
            result.TomTat = entity.TomTat;
            result.TomTatNgan = entity.TomTatNgan;
            result.Trailer = entity.Trailer;
            //result.Theaters = entity.Theaters;
            result.OnDVD = entity.OnDVD;
            result.Time = entity.Time;
            //result.Studio = entity.Studio;
            result.Rating = entity.Rating;
            db.SaveChanges();
            return result.MaPhim;
        }
        public void RemoveAllGenreTVShowByTVShowID(int movieID)
        {
            var listResult = GetGenreTVShowByTVShowID(movieID);
            foreach (var item in listResult)
            {
                db.GENREPHIMs.Remove(item);
            }
        }

        public void RemoveAllDirectorTVShowByTVShowID(int movieID)
        {
            var listResult = GetDirectorTVShowByTVShowID(movieID);
            foreach (var item in listResult)
            {
                db.DAODIENPHIMs.Remove(item);
            }
        }

        public string GetTVShowNameByImageID(int id)
        {
            int movieid = (int)db.ANHPHIMs.Find(id).MaPhim;

            return db.PHIMs.Find(movieid).TenPhim;
        }

        public void RemoveAllWrittenTVShowByTVShowID(int movieID)
        {
            var listResult = GetWrittenTVShowByTVShowID(movieID);
            foreach (var item in listResult)
            {
                db.KICHBANPHIMs.Remove(item);
            }
        }
        public List<DIENVIENPHIM> GetCastTVShowByTVShowID(int id)
        {
            List<DIENVIENPHIM> result = db.DIENVIENPHIMs.Where(n => n.MaPhim == id).ToList();
            return result;
        }
        public string GetCharacterNameByCastID(int id)
        {
            return db.DIENVIENPHIMs.SingleOrDefault(n => n.ID == id).TenNhanVat;
        }
        public int GetCastTVShowIDByTVShowIDAndPersonID(int movieID, int id)
        {
            return db.DIENVIENPHIMs.SingleOrDefault(n => n.MaDienVien == id && n.MaPhim == movieID).ID;
        }
        public bool DeleteCast(int id)
        {
            db.DIENVIENPHIMs.Remove(db.DIENVIENPHIMs.Find(id));
            db.SaveChanges();
            return true;
        }

        public string GetCharacterNameByTVShowIDAndPersonID(int id, int maDienVien)
        {
            return db.DIENVIENPHIMs.SingleOrDefault(n => n.MaDienVien == maDienVien && n.MaPhim == id).TenNhanVat;
        }

        public void EditCastNameByID(int id, string name)
        {
            var result = db.DIENVIENPHIMs.Find(id);
            result.TenNhanVat = name;
            db.SaveChanges();
        }
        public int InsertNewTVShowInfo(int id, int network, string PremiereDate, int sotap)
        {
            TVSHOW result = new TVSHOW();
            result.MaPhim = id;
            result.SoTap = sotap;
            result.Network = network;
            result.PremiereDate = PremiereDate;
            db.TVSHOWs.Add(result);
            db.SaveChanges();
            return result.ID;
        }

        public int GetTVShowNetworkByTVShowID(int id)
        {
            return (int)db.TVSHOWs.SingleOrDefault(n => n.MaPhim == id).Network;
        }

        public string GetPremiereDateByTVShowID(int id)
        {
            return db.TVSHOWs.SingleOrDefault(n => n.MaPhim == id).PremiereDate;
        }

        public int UpdateInfo(TVSHOW tvshow)
        {
            TVSHOW result = db.TVSHOWs.SingleOrDefault(n => n.MaPhim == tvshow.MaPhim);
            result.Network = tvshow.Network;
            result.PremiereDate = tvshow.PremiereDate;
            db.SaveChanges();
            return result.ID;
        }

        public List<TVSHOWEP> GetAllEpsByTVShowID(int id)
        {
            return db.TVSHOWEPs.Where(n => n.MaPhim == id).ToList();
        }
        public TVSHOWEP GetEpsByEpsID(int id)
        {
            return db.TVSHOWEPs.Find(id);
        }
        public bool DeleteLastEpsByEpsID(int id)
        {
            db.TVSHOWEPs.Remove(GetEpsByEpsID(id));
            db.SaveChanges();
            return true;
        }

        public bool EditEpsNameByEpsID(int id, string name)
        {
            var result = GetEpsByEpsID(id);
            result.TenTap = name;
            db.SaveChanges();
            return true;
        }

        public int GetCountEpsByTVShowID(int id)
        {
            int numbereps = 0;
            numbereps = db.TVSHOWEPs.Count(n => n.MaPhim == id);
            return numbereps;
        }
        private List<TVSHOW> PremiereDate()
        {
            return db.TVSHOWs.ToList();
        }
        public IEnumerable<PHIM> OpeningThisMonth(int page, int pageSize, string searchString)
        {
            string now = DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-");
            string month = now.Split('-')[1];
            string year = now.Split('-')[2];
            var temp = PremiereDate();
            List<int> id = new List<int>();
            foreach (var item in temp)
            {
                if (item.PremiereDate.Replace("/", "-").Split('-')[1] == month && item.PremiereDate.Replace("/", "-").Split('-')[2] == year)
                {
                    id.Add((int)item.MaPhim);
                }
            }
            List<PHIM> result = new List<PHIM>();
            foreach (var item in id)
            {
                result.Add(db.PHIMs.Find(item));
            }

            IEnumerable<PHIM> finalResult = result.AsEnumerable();
            if (!string.IsNullOrEmpty(searchString))
            {
                finalResult = finalResult.Where(n => n.TenPhim.Contains(searchString));
            }
            finalResult = finalResult.OrderByDescending(n => n.TenPhim).ToPagedList(page, pageSize);
            return finalResult;
        }

        public IEnumerable<PHIM> ComingSoon(int page, int pageSize, string searchString)
        {
            string now = DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-");
            string month = now.Split('-')[1];
            string year = now.Split('-')[2];
            var temp = PremiereDate();
            List<int> id = new List<int>();
            foreach (var item in temp)
            {
                if (int.Parse(item.PremiereDate.Replace("/", "-").Split('-')[1]) >= int.Parse(month) && int.Parse(item.PremiereDate.Replace("/", "-").Split('-')[2]) >= int.Parse(year))
                {
                    id.Add((int)item.MaPhim);
                }
            }
            List<PHIM> result = new List<PHIM>();
            foreach (var item in id)
            {
                result.Add(db.PHIMs.Find(item));
            }

            IEnumerable<PHIM> finalResult = result.AsEnumerable();
            if (!string.IsNullOrEmpty(searchString))
            {
                finalResult = finalResult.Where(n => n.TenPhim.Contains(searchString));
            }
            finalResult = finalResult.OrderByDescending(n => n.TenPhim).ToPagedList(page, pageSize);
            return finalResult;
        }

        public IEnumerable<PHIM> Fresh(int page, int pageSize, string searchString)
        {
            var result = GetAllTVShow().Where(n => n.TenPhim.Contains("") && n.LemonCatScore > 65);
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenPhim.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.TenPhim).ToPagedList(page, pageSize);
            return result;
        }

        public IEnumerable<PHIM> AllTVShow(int page, int pageSize, string searchString)
        {
            var result = GetAllTVShow().Where(n => n.TenPhim.Contains(""));
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.Where(n => n.TenPhim.Contains(searchString));
            }
            result = result.OrderByDescending(n => n.TenPhim).ToPagedList(page, pageSize);
            return result;
        }

        public List<PHIM> GetBySearchString(string searchString)
        {
            return db.PHIMs.Where(n => n.IsTVShow == 1 && n.TenPhim.Contains(searchString)).ToList();
        }
    }
}