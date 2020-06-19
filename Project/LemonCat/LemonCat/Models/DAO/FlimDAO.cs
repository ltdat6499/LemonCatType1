using LemonCat.Models.EF;
using Model.DAO;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class FlimDAO
    {
        private static FlimDAO instance;
        LemonCatEntities db = null;
        public FlimDAO()
        {
            db = new LemonCatEntities();
        }

        public static FlimDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new FlimDAO();
                return FlimDAO.instance;
            }
            private set
            {
                FlimDAO.instance = value;
            }
        }
        public List<string> GetListTagByReviewID(int id)
        {
            List<string> result = new List<string>();
            int movieID = (int)db.BAIVIETDANHGIAs.Find(id).MaPhim;
            var list = db.TAGS_MOVIE.Where(n => n.MaPhim == id);
            foreach (var item in list)
            {
                result.Add(item.Tag);
            }
            return result;
        }
        public List<PHIM> GetAllFlim()
        {
            return db.PHIMs.OrderBy(n=>n.IsTVShow).ToList();
        }

        public bool IsReview(int account, int flimid)
        {
            int count = db.BAIVIETDANHGIAs.Count(n => n.MaPhim == flimid && n.MaTK == account);
            if (count > 0)
                return true;
            return false;
        }

        public int InsertReview(int flimid, string noiDungBinhLuan, string noiDungNgan, int finalScore, string date, int account)
        {
            BAIVIETDANHGIA result = new BAIVIETDANHGIA();
            result.MaPhim = flimid;
            result.NoiDungBinhLuan = noiDungBinhLuan;
            result.NoiDungNgan = noiDungNgan;
            result.Score = finalScore;
            result.NgayBinhLuan = date;
            result.MaTK = account;
            result.DiemDanhGia = 0;
            result.Status = true;
            db.BAIVIETDANHGIAs.Add(result);
            db.SaveChanges();

            TriggerSetFlimScore();
            return result.MaBaiViet;
        }

        public List<BAIVIETDANHGIA> GetTopReviewByFlimID(int id)
        {
            int count = db.BAIVIETDANHGIAs.Where(n => n.MaPhim == id).Count();
            if (count > 3)
            {
                count = 3;
            }
            return db.BAIVIETDANHGIAs.Where(n => n.MaPhim == id).Take(count).ToList();
        }

        public int CountReviewByFlimID(int id)
        {
            return db.BAIVIETDANHGIAs.Count(n => n.MaPhim == id);
        }

        public List<BAIVIETDANHGIA> GetAllReview()
        {
            return db.BAIVIETDANHGIAs.ToList();
        }
        public List<BAIVIETDANHGIA> GetReviewByFlimID(int id)
        {
            return db.BAIVIETDANHGIAs.Where(n=>n.MaPhim == id).ToList();
        }
        private void TriggerSetFlimScore()
        {
            var listFlim = GetAllFlim();
            foreach (var item in listFlim)
            {
                int UserScore = 0;
                int CountUser = 0;
                int LemonCatScore = 0;
                int CountLemonCat = 0;
                var listReview = GetReviewByFlimID(item.MaPhim);
                foreach (var subitem in listReview)
                {
                    int accountPosition = UserDAO.Instance.GetPositionByID((int)subitem.MaTK);
                    if (accountPosition == 1)
                    {
                        UserScore += (int)subitem.Score;
                        CountUser++;
                    }
                    else
                    {
                        LemonCatScore += (int)subitem.Score;
                        CountLemonCat++;
                    }
                }
                if (CountUser > 0)
                {
                    var result = db.SCOREs.SingleOrDefault(n => n.MaPhim == item.MaPhim);
                    result.CountUser = CountUser;
                    result.UserScore = UserScore / CountUser;
                    db.SaveChanges();
                }
                else
                {
                    var result = db.SCOREs.SingleOrDefault(n => n.MaPhim == item.MaPhim);
                    result.CountUser = 0;
                    result.UserScore = 0;
                    db.SaveChanges();
                }
                if (CountLemonCat > 0)
                {
                    var result = db.SCOREs.SingleOrDefault(n => n.MaPhim == item.MaPhim);
                    result.CountLemonCat = CountLemonCat;
                    result.LemonCatScore = LemonCatScore / CountLemonCat;
                    db.SaveChanges();
                }
                else
                {
                    var result = db.SCOREs.SingleOrDefault(n => n.MaPhim == item.MaPhim);
                    result.CountLemonCat = 0;
                    result.LemonCatScore = 0;
                    db.SaveChanges();
                }
            }
        }

        public bool ChangeStatusReviewByID(int id)
        {
            var result = GetReviewByID(id);
            result.Status = !result.Status;
            db.SaveChanges();
            return (bool)result.Status;
        }

        public List<BAIVIET_SCORE> GetListReviewScoreByReviewID(int id)
        {
            return db.BAIVIET_SCORE.Where(n => n.MaBaiViet == id).ToList();
        }

        public void UpdateReview(int id, string noiDungBinhLuan, string noiDungNgan, int finalScore, string date)
        {
            var result = GetReviewByID(id);
            result.NoiDungBinhLuan = noiDungBinhLuan;
            result.NoiDungNgan = noiDungNgan;
            result.Score = finalScore;
            result.NgayBinhLuan = date;
            db.SaveChanges();
        }

        public bool LikeReviewByUserIDAndReviewID(int id, int userID)
        {
            int count = db.BAIVIET_SCORE.Count(n => n.MaTK == userID && n.MaBaiViet == id);
            if (count == 0)
            {
                BAIVIET_SCORE result = new BAIVIET_SCORE();
                result.MaBaiViet = id;
                result.MaTK = userID;
                result.Mark = true;
                db.BAIVIET_SCORE.Add(result);
                db.SaveChanges();

                ReviewScoreTrigger(id);
                return true;
            }
            else
            {
                var result = db.BAIVIET_SCORE.SingleOrDefault(n => n.MaBaiViet == id && n.MaTK == userID);
                result.Mark = !result.Mark;
                db.SaveChanges();

                ReviewScoreTrigger(id);
                return (bool)result.Mark;
            }
        }

        public List<BAIVIET_ROOTCOMMENT> GetListRootCommentByReviewID(int id)
        {
            return db.BAIVIET_ROOTCOMMENT.Where(n => n.MaBaiViet == id).ToList();
        }
        public List<BAIVIET_ROOTCOMMENT> GetListRootCommentByReviewIDClient(int id)
        {
            return db.BAIVIET_ROOTCOMMENT.Where(n => n.MaBaiViet == id && n.Status == true).ToList();
        }

        private void ReviewScoreTrigger(int id)
        {
            var list = db.BAIVIET_SCORE.Where(n => n.MaBaiViet == id);
            int count = 0;
            foreach (var item in list)
            {
                if (item.Mark == true)
                    count++;
            }
            var result = GetReviewByID(id);
            result.DiemDanhGia = count;
            db.SaveChanges();
        }

        public int CountLikeRootByRootID(int id)
        {
            return db.BAIVIET_ROOTSCORE.Count(n => n.IDRoot == id && n.Mark == true);
        }
        public int CountRootByReviewID(int id)
        {
            return db.BAIVIET_ROOTCOMMENT.Where(n=>n.MaBaiViet == id).Count();
        }
        public int CountChildByRootID(int id)
        {
            return db.BAIVIET_CHILDCOMMENT.Count(n => n.IDRoot == id);
        }

        public bool GetRootCommentMarkByUserID(int userID, int id)
        {
            int count = db.BAIVIET_ROOTSCORE.Count(n => n.IDRoot == id && n.MaTK == userID);
            if (count == 0)
                return false;
            else
            {
                var result = db.BAIVIET_ROOTSCORE.SingleOrDefault(n => n.IDRoot == id && n.MaTK == userID).Mark;
                return (bool)result;
            }
        }

        public void DeleteAllRootScoreByRootID(int id)
        {
            var result = db.BAIVIET_ROOTSCORE.Where(n => n.IDRoot == id);
            foreach (var item in result)
            {
                db.BAIVIET_ROOTSCORE.Remove(item);
            }
            db.SaveChanges();
        }
        public void DeleteAllReviewScoreByReviewID(int id)
        {
            var result = db.BAIVIET_SCORE.Where(n => n.MaBaiViet == id);
            foreach (var item in result)
            {
                db.BAIVIET_SCORE.Remove(item);
            }
            db.SaveChanges();
        }

        public int WriteComment(int id, string content, int account, string date)
        {
            BAIVIET_ROOTCOMMENT result = new BAIVIET_ROOTCOMMENT();
            result.MaBaiViet = id;
            result.NoiDungBinhLuan = content;
            result.DiemDanhGiaBinhLuan = 0;
            result.MaTK = account;
            result.NgayBinhLuan = date;
            result.Status = true;
            db.BAIVIET_ROOTCOMMENT.Add(result);
            db.SaveChanges();
            return result.ID;
        }

        public List<BAIVIET_CHILDCOMMENT> GetListChildByRootID(int id)
        {
            return db.BAIVIET_CHILDCOMMENT.Where(n => n.IDRoot == id).ToList();
        }
        public List<BAIVIET_CHILDCOMMENT> GetListChildByRootIDClient(int id)
        {
            return db.BAIVIET_CHILDCOMMENT.Where(n => n.IDRoot == id && n.Status == true).ToList();
        }

        public int WriteChildComment(int id, string content, int account, string date)
        {
            BAIVIET_CHILDCOMMENT result = new BAIVIET_CHILDCOMMENT();
            result.IDRoot = id;
            result.NoiDungBinhLuan = content;
            result.MaTK = account;
            result.NgayBinhLuan = date;
            result.Status = true;
            db.BAIVIET_CHILDCOMMENT.Add(result);
            db.SaveChanges();
            return result.ID;
        }
        public BAIVIET_CHILDCOMMENT GetChildByChildID(int id)
        {
            return db.BAIVIET_CHILDCOMMENT.Find(id);
        }
        public void DeleteChildByChildID(int id)
        {
            db.BAIVIET_CHILDCOMMENT.Remove(GetChildByChildID(id));
            db.SaveChanges();
        }

        public void DeleteRootByRootID(int id)
        {
            var root = db.BAIVIET_ROOTCOMMENT.Find(id);
            //Delete RootScore
            DeleteAllRootScoreByRootID(id);
            //Delete ChildComment
            DeleteAllChildCommentByRootID(id);
            db.BAIVIET_ROOTCOMMENT.Remove(root);
            db.SaveChanges();
        }
        public BAIVIET_ROOTCOMMENT GetRootByRootID(int id)
        {
            return db.BAIVIET_ROOTCOMMENT.Find(id);
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

        public bool LikeRootByUserIDAndRootID(int id, int userID)
        {
            int count = db.BAIVIET_ROOTSCORE.Count(n => n.MaTK == userID && n.IDRoot == id);
            if (count == 0)
            {
                BAIVIET_ROOTSCORE result = new BAIVIET_ROOTSCORE();
                result.IDRoot = id;
                result.MaTK = userID;
                result.Mark = true;
                db.BAIVIET_ROOTSCORE.Add(result);
                db.SaveChanges();

                RootScoreTrigger(id);
                return true;
            }
            else
            {
                var result = db.BAIVIET_ROOTSCORE.SingleOrDefault(n => n.IDRoot == id && n.MaTK == userID);
                result.Mark = !result.Mark;
                db.SaveChanges();

                RootScoreTrigger(id);
                return (bool)result.Mark;
            }
        }

        private void RootScoreTrigger(int id)
        {
            var list = db.BAIVIET_ROOTSCORE.Where(n => n.IDRoot == id);
            int count = 0;
            foreach (var item in list)
            {
                if (item.Mark == true)
                    count++;
            }
            var result = GetRootByRootID(id);
            result.DiemDanhGiaBinhLuan = count;
            db.SaveChanges();
        }

        public void DeleteAllChildCommentByRootID(int id)
        {
            var result = GetListChildByRootID(id);
            foreach (var item in result)
            {
                db.BAIVIET_CHILDCOMMENT.Remove(item);
            }
            db.SaveChanges();
        }
        public void DeleteAllRootByReviewID(int id)
        {
            var result = db.BAIVIET_ROOTCOMMENT.Where(n => n.MaBaiViet == id);
            foreach (var item in result)
            {
                db.BAIVIET_ROOTCOMMENT.Remove(item);
            }
            db.SaveChanges();
        }
        public BAIVIETDANHGIA GetReviewByFlimIDAndUserID(int movieID, int AccID)
        {
            return db.BAIVIETDANHGIAs.Where(n => n.MaPhim == movieID && n.MaTK == AccID).FirstOrDefault();
        }
        public BAIVIETDANHGIA GetReviewByID(int id)
        {
            return db.BAIVIETDANHGIAs.SingleOrDefault(n => n.MaBaiViet == id);
        }
        public IEnumerable<BAIVIETDANHGIA> GetReviewByUserIDSortByIndex(int id, int page, int pagesize)
        {
            return db.BAIVIETDANHGIAs.Where(n => n.MaTK == id && n.Status == true).OrderByDescending(n => n.MaPhim).ToPagedList(page, pagesize);
        }
        public void DeleteReviewByReviewID(int id)
        {
            var result = GetReviewByID(id);
            db.BAIVIETDANHGIAs.Remove(result);
            db.SaveChanges();
        }
        public void Delete(int id)
        {
            //Delete ReviewScore
            DeleteAllReviewScoreByReviewID(id);

            var rootlist = db.BAIVIET_ROOTCOMMENT.Where(n => n.MaBaiViet == id);
            foreach (var item in rootlist)
            {
                //Delete RootScore
                DeleteAllRootScoreByRootID(item.ID);
                //Delete ChildComment
                DeleteAllChildCommentByRootID(item.ID);
            }
            //Delete RootComment 
            DeleteAllRootByReviewID(id);

            //Delete Review
            DeleteReviewByReviewID(id);

            TriggerSetFlimScore();
        }

        public bool GetReviewMarkByUserID(int userID, int id)
        {
            int count = db.BAIVIET_SCORE.Count(n => n.MaBaiViet == id && n.MaTK == userID);
            if (count == 0)
                return false;
            else
            {
                var result = db.BAIVIET_SCORE.SingleOrDefault(n => n.MaBaiViet == id && n.MaTK == userID).Mark;
                return (bool)result;
            }
        }

        public string GetImageToShowByReviewID(int id)
        {
            int movieID = (int)db.BAIVIETDANHGIAs.Find(id).MaPhim;
            return MovieDAO.Instance.GetSomeImageMovieByMovieID(movieID)[0].Anh;
        }

        public IEnumerable<BAIVIETDANHGIA> GetHotReviewClient(int page, int pageSize, string searchString)
        {
            var temp = GetAllFlim();
            List<int> id = new List<int>();
            foreach (var item in temp)
            {
                if (item.TenPhim.Contains(searchString))
                {
                    id.Add((int)item.MaPhim);
                }
            }
            id = id.Distinct().ToList();
            List<BAIVIETDANHGIA> result = new List<BAIVIETDANHGIA>();
            foreach (int item in id)
            {
                result.AddRange(db.BAIVIETDANHGIAs.Where(n => n.MaPhim == item));
            }
            var listReview = GetAllReview();
            foreach (var item in listReview)
            {
                if (item.NoiDungNgan.Contains(searchString) && !id.Contains((int)item.MaPhim))
                {
                    result.Add(item);
                }
            }
            IEnumerable<BAIVIETDANHGIA> finalResult = result.AsEnumerable();
            finalResult = finalResult.OrderByDescending(n => n.DiemDanhGia).ToPagedList(page, pageSize);
            return finalResult;
        }
        public IEnumerable<BAIVIETDANHGIA> GetAllReviewClient(int page, int pageSize, string searchString)
        {
            var temp = GetAllFlim();
            List<int> id = new List<int>();
            foreach (var item in temp)
            {
                if (item.TenPhim.Contains(searchString))
                {
                    id.Add((int)item.MaPhim);
                }
            }
            id = id.Distinct().ToList();
            List<BAIVIETDANHGIA> result = new List<BAIVIETDANHGIA>();
            foreach (int item in id)
            {
                result.AddRange(db.BAIVIETDANHGIAs.Where(n => n.MaPhim == item));
            }
            var listReview = GetAllReview();
            foreach (var item in listReview)
            {
                if (item.NoiDungNgan.Contains(searchString) && !id.Contains((int)item.MaPhim))
                {
                    result.Add(item);
                }
            }
            IEnumerable<BAIVIETDANHGIA> finalResult = result.AsEnumerable();
            finalResult = finalResult.OrderByDescending(n => n.DiemDanhGia).ToPagedList(page, pageSize);
            return finalResult;
        }

        public List<BAIVIETDANHGIA> GetBySearchString(string searchString)
        {
            return db.BAIVIETDANHGIAs.Where(n => n.NoiDungNgan.Contains(searchString)).ToList();
        }
    }
}