using LemonCat.Models.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class NewsDAO
    {
        private static NewsDAO instance;
        LemonCatEntities db = null;
        public NewsDAO()
        {
            db = new LemonCatEntities();
        }

        public static NewsDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new NewsDAO();
                return NewsDAO.instance;
            }
            private set
            {
                NewsDAO.instance = value;
            }
        }
        public List<NewsScore> GetListNewsScoreByNewsID(int id)
        {
            return db.NewsScores.Where(n => n.NewsID == id).ToList();
        }
        public List<NEW> GetAllNews()
        {
            return db.NEWs.ToList();
        }
        public int CreateNew(string tieude, int user, int newcategory, string noidung, string noidungngan, string date)
        {
            NEW result = new NEW();
            result.TuaNews = tieude;
            result.MaTK = user;
            result.LoaiNew = newcategory;
            result.NoiDung = noidung;
            result.NoiDungNgan = noidungngan;
            result.NgayDang = date;
            result.DiemDanhGia = 0;
            result.Status = true;
            db.NEWs.Add(result);
            db.SaveChanges();
            return result.MaNew;
        }
        public NEW GetNews(int id)
        {
            return db.NEWs.Find(id);
        }
        public int EditNew(int idNews, string tieude, int newcategory, string noidung, string noidungngan, string date)
        {
            var result = GetNews(idNews);
            result.TuaNews = tieude;
            result.LoaiNew = newcategory;
            result.NoiDung = noidung;
            result.NoiDungNgan = noidungngan;
            result.NgayDang = date;
            db.SaveChanges();
            return result.MaNew;
        }

        public bool GetNewsMarkByUserID(int userID, int maNew)
        {
            int count = db.NewsScores.Count(n => n.NewsID == maNew && n.MaTK == userID);
            if (count == 0)
                return false;
            else
            {
                var result = db.NewsScores.SingleOrDefault(n => n.NewsID == maNew && n.MaTK == userID).Score;
                return (bool)result;
            }
        }

        public List<LOAINEW> GetAllType()
        {
            return db.LOAINEWs.ToList();
        }
        public void InsertFlimToNews(int newsID, int FlimID)
        {
            CHITIETPHIMNEW result = new CHITIETPHIMNEW();
            result.MaNew = newsID;
            result.MaPhim = FlimID;
            db.CHITIETPHIMNEWs.Add(result);
            db.SaveChanges();
        }
        public void InsertPersontoNews(int newsID, int PersonID)
        {
            CHITIETDIENVIENNEW result = new CHITIETDIENVIENNEW();
            result.MaNew = newsID;
            result.MaDienVien = PersonID;
            db.CHITIETDIENVIENNEWs.Add(result);
            db.SaveChanges();
        }
        public List<CHITIETPHIMNEW> GetAllFlimByNewsID(int id)
        {
            return db.CHITIETPHIMNEWs.Where(n => n.MaNew == id).ToList();
        }
        public List<CHITIETDIENVIENNEW> GetAllPersonByNewsID(int id)
        {
            return db.CHITIETDIENVIENNEWs.Where(n => n.MaNew == id).ToList();
        }
        public void RemoveFlimNews(int newID)
        {
            var list = db.CHITIETPHIMNEWs.Where(n => n.MaNew == newID);
            foreach (var item in list)
            {
                db.CHITIETPHIMNEWs.Remove(item);
            }
            db.SaveChanges();
        }
        public List<RootCommentNew> GetListRootCommentByNewsID(int id)
        {
            return db.RootCommentNews.Where(n => n.MaNew == id).ToList();
        }
        public List<RootCommentNew> GetListRootCommentByNewsIDClient(int id)
        {
            return db.RootCommentNews.Where(n => n.MaNew == id && n.Status == true).ToList();
        }
        public void RemovePersontoNews(int newID)
        {
            var list = db.CHITIETDIENVIENNEWs.Where(n => n.MaNew == newID);
            foreach (var item in list)
            {
                db.CHITIETDIENVIENNEWs.Remove(item);
            }
            db.SaveChanges();
        }
        public void DeleteAllRootScoreByRootID(int id)
        {
            var result = db.RootNewScores.Where(n=>n.RootCommentNewID == id);
            foreach (var item in result)
            {
                db.RootNewScores.Remove(item);
            }
            db.SaveChanges();
        }
        public void DeleteAllNewsScoreByNewsID(int id)
        {
            var result = db.NewsScores.Where(n => n.NewsID == id);
            foreach (var item in result)
            {
                db.NewsScores.Remove(item);
            }
            db.SaveChanges();
        }
        public void DeleteAllChildCommentByRootID(int id)
        {
            var result = GetListChildByRootID(id);
            foreach (var item in result)
            {
                db.ChildCommentNews.Remove(item);
            }
            db.SaveChanges();
        }
        public void DeleteAllRootByNewsID(int id)
        {
            var result = db.RootCommentNews.Where(n => n.MaNew == id);
            foreach (var item in result)
            {
                db.RootCommentNews.Remove(item);
            }
            db.SaveChanges();
        }
        public void DeleteNewsByNewsID(int id)
        {
            var result = GetNews(id);
            db.NEWs.Remove(result);
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            //Delete NewScore
            DeleteAllNewsScoreByNewsID(id);

            var rootlist = db.RootCommentNews.Where(n => n.MaNew == id);
            foreach (var item in rootlist)
            {
                //Delete RootScore
                DeleteAllRootScoreByRootID(item.ID);
                //Delete ChildComment
                DeleteAllChildCommentByRootID(item.ID);
            }
            //Delete RootComment 
            DeleteAllRootByNewsID(id);

            //Delete chitietphimnew
            RemoveFlimNews(id);
            //Delete chitietdienviennew
            RemovePersontoNews(id);

            //Delete new
            DeleteNewsByNewsID(id);
        }

        public bool ChangeStatusNewsByNewsID(int id)
        {
            var result = GetNews(id);
            result.Status = !result.Status;
            db.SaveChanges();
            return (bool)result.Status;
        }

        public ChildCommentNew GetChildByChildID(int id)
        {
            return db.ChildCommentNews.Find(id);
        }

        public void DeleteRootByRootID(int id)
        {
            var root = db.RootCommentNews.Find(id);
            //Delete RootScore
            DeleteAllRootScoreByRootID(id);
            //Delete ChildComment
            DeleteAllChildCommentByRootID(id);
            db.RootCommentNews.Remove(root);
            db.SaveChanges();
        }
        public RootCommentNew GetRootByRootID(int id)
        {
            return db.RootCommentNews.Find(id);
        }

        public bool ChangeStatusChildByChildID(int id)
        {
            var result = GetChildByChildID(id);
            result.Status = !result.Status;
            db.SaveChanges();
            return (bool)result.Status;
        }

        public bool ChangeStatusRootByRootID(int id)
        {
            var result = GetRootByRootID(id);
            result.Status = !result.Status;
            db.SaveChanges();
            return (bool)result.Status;
        }

        public int WriteComment(int id, string content, int account, string date)
        {
            RootCommentNew result = new RootCommentNew();
            result.MaNew = id;
            result.NoiDungBinhLuan = content;
            result.DiemDanhGia = 0;
            result.MaTK = account;
            result.NgayBinhLuan = date;
            result.Status = true;
            db.RootCommentNews.Add(result);
            db.SaveChanges();
            return result.ID;
        }

        public void DeleteChildByChildID(int id)
        {
            db.ChildCommentNews.Remove(GetChildByChildID(id));
            db.SaveChanges();
        }

        public int WriteChildComment(int id, string content, int account, string date)
        {
            ChildCommentNew result = new ChildCommentNew();
            result.RootCommentNewID = id;
            result.NoiDungBinhLuan = content;
            result.MaTK = account;
            result.NgayBinhLuan = date;
            result.Status = true;
            db.ChildCommentNews.Add(result);
            db.SaveChanges();
            return result.ID;
        }

        public List<ChildCommentNew> GetListChildByRootID(int id)
        {
            return db.ChildCommentNews.Where(n => n.RootCommentNewID == id).ToList();
        }
        public List<ChildCommentNew> GetListChildByRootIDClient(int id)
        {
            return db.ChildCommentNews.Where(n => n.RootCommentNewID == id && n.Status == true).ToList();
        }

        public bool GetRootCommentMarkByUserID(int userID, int id)
        {
            int count = db.RootNewScores.Count(n => n.RootCommentNewID == id && n.MaTK == userID);
            if (count == 0)
                return false;
            else
            {
                var result = db.RootNewScores.SingleOrDefault(n => n.RootCommentNewID == id && n.MaTK == userID).Score;
                return (bool)result;
            }
        }
        public void RootScoreTrigger(int RootID)
        {
            var list = db.RootNewScores.Where(n => n.RootCommentNewID == RootID);
            int count = 0;
            foreach (var item in list)
            {
                if (item.Score == true)
                    count++;
            }
            var result = GetRootByRootID(RootID);
            result.DiemDanhGia = count;
            db.SaveChanges();
        }
        public void NewsScoreTrigger(int ID)
        {
            var list = db.NewsScores.Where(n => n.NewsID == ID);
            int count = 0;
            foreach (var item in list)
            {
                if (item.Score == true)
                    count++;
            }
            var result = GetNews(ID);
            result.DiemDanhGia = count;
            db.SaveChanges();
        }

        public int CountLikeRootByRootID(int id)
        {
            return db.RootNewScores.Count(n=>n.RootCommentNewID == id && n.Score == true);
        }

        public int CountChildByRootID(int id)
        {
            return db.ChildCommentNews.Count(n => n.RootCommentNewID == id);
        }

        public bool LikeRootByUserIDAndRootID(int id, int userID)
        {
            int count = db.RootNewScores.Count(n => n.MaTK == userID && n.RootCommentNewID == id);
            if (count == 0)
            {
                RootNewScore result = new RootNewScore();
                result.RootCommentNewID = id;
                result.MaTK = userID;
                result.Score = true;
                db.RootNewScores.Add(result);
                db.SaveChanges();

                RootScoreTrigger(id);
                return true;
            }
            else
            {
                var result = db.RootNewScores.SingleOrDefault(n => n.RootCommentNewID == id && n.MaTK == userID);
                result.Score = !result.Score;
                db.SaveChanges();

                RootScoreTrigger(id);
                return (bool)result.Score;
            }
        }

        public bool LikeNewsByUserIDAndNewsID(int id, int userID)
        {
            int count = db.NewsScores.Count(n => n.MaTK == userID && n.NewsID == id);
            if (count == 0)
            {
                NewsScore result = new NewsScore();
                result.NewsID = id;
                result.MaTK = userID;
                result.Score = true;
                db.NewsScores.Add(result);
                db.SaveChanges();

                NewsScoreTrigger(id);
                return true;
            }
            else
            {
                var result = db.NewsScores.SingleOrDefault(n => n.NewsID == id && n.MaTK == userID);
                result.Score = !result.Score;
                db.SaveChanges();

                NewsScoreTrigger(id);
                return (bool)result.Score;
            }
        }
        public IEnumerable<NEW> GetNewsByUserID(int id, int page, int pagesize)
        {
            return db.NEWs.Where(n=>n.MaTK == id && n.Status == true).OrderByDescending(n => n.LoaiNew).ToPagedList(page, pagesize);
        }

        public string GetImageToShowByNewsID(int id)
        {
            int count1 = db.CHITIETDIENVIENNEWs.Where(n => n.MaNew == id).Count();
            int count2 = db.CHITIETPHIMNEWs.Where(n => n.MaNew == id).Count();
            
            if (count1 > 0)
            {
                var P = db.CHITIETDIENVIENNEWs.Where(n => n.MaNew == id).FirstOrDefault();
                return PersonDAO.Instance.GetSomeImagePersonByPersonID((int)P.MaDienVien)[0].Anh;
            }
            else if (count2 > 0)
            {
                var P = db.CHITIETPHIMNEWs.Where(n => n.MaNew == id).FirstOrDefault();
                return MovieDAO.Instance.GetSomeImageMovieByMovieID((int)P.MaPhim)[0].Anh;
            }
            else
            {
                return "~\\Areas\\Admin\\pppp.jpg";
            }
        }

        public int CountRoot(int id)
        {
            return db.RootCommentNews.Where(n => n.MaNew == id).Count();
        }

        public IEnumerable<NEW> GetHotNewsClient(int page, int pageSize, string searchString)
        {
            var temp = GetAllNews();
            IEnumerable<NEW> finalResult = temp.Where(n => n.NoiDungNgan.Contains(searchString)).OrderByDescending(n => n.DiemDanhGia).AsEnumerable();
            finalResult = finalResult.OrderByDescending(n => n.DiemDanhGia).ToPagedList(page, pageSize);
            return finalResult;
        }

        public IEnumerable<NEW> GetAllNewsClient(int page, int pageSize, string searchString)
        {
            var temp = GetAllNews();
            IEnumerable<NEW> finalResult = temp.Where(n => n.NoiDungNgan.Contains(searchString)).OrderByDescending(n => n.DiemDanhGia).AsEnumerable();
            finalResult = finalResult.OrderByDescending(n => n.DiemDanhGia).ToPagedList(page, pageSize);
            return finalResult;
        }

        public List<NEW> GetBySearchString(string searchString)
        {
            return db.NEWs.Where(n => n.TuaNews.Contains(searchString)).ToList();
        }
    }
}