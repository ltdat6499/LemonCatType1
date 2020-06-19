using LemonCat.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class YearDataDAO
    {
        private static YearDataDAO instance;
        LemonCatEntities db = null;
        public YearDataDAO()
        {
            db = new LemonCatEntities();
        }

        public static YearDataDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new YearDataDAO();
                return YearDataDAO.instance;
            }
            private set
            {
                YearDataDAO.instance = value;
            }
        }

        private int NormalUserCount()
        {
            return db.TAIKHOANs.Where(n => n.ChucVu == 1).Count();
        }
        private int NonNormalUserCount()
        {
            return db.TAIKHOANs.Where(n => n.ChucVu > 1).Count();
        }
        public int MovieCount()
        {
            return db.PHIMs.Where(n => (int)n.IsTVShow == 0).Count();
        }
        public int TVCount()
        {
            return db.TVSHOWs.Count();
        }
        public int UserCount()
        {
            return db.TAIKHOANs.Count();
        }
        public int NewsCount()
        {
            return db.NEWs.Count();
        }
        public int ReviewCount()
        {
            return db.BAIVIETDANHGIAs.Count();
        }
        private int TicketRevenue()
        {
            var list = db.ORDERSEATs.ToList();
            int result = 0;
            foreach (var item in list)
            {
                result += (int)item.TongTien;
            }
            return result;
        }
        private int DVDRevenue()
        {
            var list = db.HOADONs.ToList();
            int result = 0;
            foreach (var item in list)
            {
                result += (int)item.TongTien;
            }
            return result;
        }
        public int PersonCount()
        {
            return db.DIENVIENs.Count();
        }
        private int DVDCount()
        {
            return db.DVDs.Count();
        }
        public YEARDATA GetData(int month, int year)
        {
            return db.YEARDATAs.Where(n => n.Month == month && n.Year == year).FirstOrDefault();
        }
        private string GetMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
                default:
                    break;
            }
            return "";
        }
        private int CreateWithZero(int month, int year)
        {
            YEARDATA result = new YEARDATA();
            result.Month = month;
            result.Year = year;
            result.TongVe = 0;
            result.TongUserLoaiMot = 0;
            result.TongUserDacBiet = 0;
            result.TongUser = 0;
            result.TongTV = 0;
            result.TongMovie = 0;
            result.TongDVD = 0;
            result.DoanhThuBanVe = 0;
            result.DoanhThuBanDVD = 0;
            result.TongReview = 0;
            result.TongNews = 0;
            result.MonthDisplay = GetMonthName(month);
            db.YEARDATAs.Add(result);
            db.SaveChanges();
            return result.ID;
        }
        private int Create(int month, int year)
        {
            YEARDATA result = new YEARDATA();
            result.Month = month;
            result.Year = year;
            result.TongVe = TicketRevenue();
            result.TongUserLoaiMot = NormalUserCount();
            result.TongUserDacBiet = NonNormalUserCount();
            result.TongUser = UserCount();
            result.TongTV = TVCount();
            result.TongMovie = MovieCount();
            result.TongDVD = DVDCount();
            result.DoanhThuBanVe = TicketRevenue();
            result.DoanhThuBanDVD = DVDRevenue();
            result.TongReview = ReviewCount();
            result.TongNews = NewsCount();
            result.MonthDisplay = GetMonthName(month);
            db.YEARDATAs.Add(result);
            db.SaveChanges();
            return result.ID;
        }
        private int Update(int month, int year)
        {
            var result = GetData(month, year);
            result.TongVe = TicketRevenue();
            result.TongUserLoaiMot = NormalUserCount();
            result.TongUserDacBiet = NonNormalUserCount();
            result.TongUser = UserCount();
            result.TongTV = TVCount();
            result.TongMovie = MovieCount();
            result.TongDVD = DVDCount();
            result.DoanhThuBanVe = TicketRevenue();
            result.DoanhThuBanDVD = DVDRevenue();
            result.TongReview = ReviewCount();
            result.TongNews = NewsCount();
            db.SaveChanges();
            return result.ID;
        }
        private int CountDataYear(int year)
        {
            return db.YEARDATAs.Where(n => n.Year == year).Count();
        }
        private int CountData(int month, int year)
        {
            return db.YEARDATAs.Where(n => n.Month == month && n.Year == year).Count();
        }
        public List<YEARDATA> GetChart(int month, int year)
        {
            if (CountData(month, year) <= 0)
            {
                Create(month, year);
            }
            Update(month, year);
            return db.YEARDATAs.Where(n => n.Year == year).ToList();
        }
    }
}