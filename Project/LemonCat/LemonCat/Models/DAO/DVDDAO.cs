using LemonCat.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class DVDDAO
    {
        private static DVDDAO instance;
        LemonCatEntities db = null;
        public DVDDAO()
        {
            db = new LemonCatEntities();
        }

        public static DVDDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new DVDDAO();
                return DVDDAO.instance;
            }
            private set
            {
                DVDDAO.instance = value;
            }
        }
        
        public List<DVD> GetAllDVD()
        {
            return db.DVDs.ToList();
        }

        public bool IsExist(int maPhim)
        {
            int count = db.DVDs.Count(n => n.MaPhim == maPhim);
            if (count > 0)
                return true;
            return false;
        }

        public void InsertDVD(int flimID, int disks, int price, int count, string date)
        {
            DVD result = new DVD();
            result.MaPhim = flimID;
            result.SoDia = disks;
            result.Gia = price;
            result.SoLuongTrongKho = count;
            result.NgayCapNhap = date;
            result.Status = true;
            db.DVDs.Add(result);
            db.SaveChanges();
        }

        public DVD GetDVDByID(int id)
        {
            return db.DVDs.Find(id);
        }

        public bool ChangeStatus(int id)
        {
            var result = db.DVDs.SingleOrDefault(n => n.MaDVD == id);
            result.Status = !result.Status;
            db.SaveChanges();
            return (bool)result.Status;
        }

        public bool Delete(int id)
        {
            //Delete in bag
            var list = db.BAGs.Where(n => n.MaDVD == id);
            foreach (var item in list)
            {
                db.BAGs.Remove(item);
            }
            db.SaveChanges();

            //Delete
            var result = GetDVDByID(id);
            db.DVDs.Remove(result);
            db.SaveChanges();
            return true;
        }

        public void UpdateDVD(int id, int flimID, int disks, int price, int count, string date)
        {
            var result = GetDVDByID(id);
            result.MaPhim = flimID;
            result.SoDia = disks;
            result.Gia = price;
            result.SoLuongTrongKho = count;
            result.NgayCapNhap = date;
            db.SaveChanges();
        }
        public List<HOADON> GetAllBill()
        {
            return db.HOADONs.ToList();
        }

        public int ChangeStatusBill(int id)
        {
            var result = db.HOADONs.SingleOrDefault(n => n.MaHoaDon == id);
            if (result.Status == 2)
            {
                result.Status = 3;
            }
            else if (result.Status == 3)
            {
                result.Status = 1;
            }
            else
            {
                result.Status = 2;
            }
            db.SaveChanges();
            return (int)result.Status;
        }

        public HOADON GetBillByID(int id)
        {
            return db.HOADONs.Find(id);
        }
        public List<CHITIETHOADON> GetChildBillByBillID(int id)
        {
            return db.CHITIETHOADONs.Where(n => n.MaHoaDon == id).ToList();
        }

        public void AcceptBillByBillID(int id)
        {
            var result = db.HOADONs.SingleOrDefault(n => n.MaHoaDon == id);
            result.Status = 1;
            db.SaveChanges();
        }
        private int GetPriceDVDbyDVDID(int id)
        {
            return (int)db.DVDs.Find(id).Gia;
        }
        public bool AddCart(int dvdID, int account)
        {
            int count = db.BAGs.Count(n => n.MaDVD == dvdID && n.MaTK == account);
            if (count == 0)
            {
                BAG result = new BAG();
                result.MaTK = account;
                result.SoLuong = 1;
                result.MaDVD = dvdID;
                result.Status = true;
                result.Gia = GetPriceDVDbyDVDID(dvdID);
                db.BAGs.Add(result);
                db.SaveChanges();
            }
            else
            {
                var result = db.BAGs.SingleOrDefault(n => n.MaTK == account && n.MaDVD == dvdID);
                result.SoLuong++;
                db.SaveChanges();
            }
            return true;
        }

        public List<HOADON> GetAllBillByUserID(int id)
        {
            return db.HOADONs.Where(n => n.MaTK == id).ToList();
        }

        public bool BagItemDeleteByID(int id)
        {
            var result = db.BAGs.Find(id);
            db.BAGs.Remove(result);
            db.SaveChanges();
            return true;
        }

        public bool ChangeBagItemStatusByItemID(int id)
        {
            var result = db.BAGs.SingleOrDefault(n => n.ID == id);
            result.Status = !result.Status;
            db.SaveChanges();
            return (bool)result.Status;
        }

        public bool ChangeAmountBagItemByItemID(int id, int? amount)
        {
            var result = db.BAGs.Find(id);
            
            if (amount == null)
            {
                result.SoLuong = 1;
                db.SaveChanges();
            }
            else
            {
                result.SoLuong = amount;
                db.SaveChanges();
            }
            return true;
        }

        public bool SubmitBag(int account, string note, string date)
        {
            int result = db.SubmitBag(account, note, date);
            if (result == -1)
            {
                return true;
            }
            return false;
        }
        private int CountProductByFlimID(int id)
        {
            return (int)db.DVDs.SingleOrDefault(n=>n.MaDVD == id).SoLuongTrongKho;
        }
        private bool IsCheckedBag(int account)
        {
            var list = db.BAGs.Where(n => n.MaTK == account);
            if (list == null)
            {
                return false;
            }
            foreach (var item in list)
            {
                if (item.SoLuong > CountProductByFlimID((int)item.MaDVD))
                    return false;
            }
            return true;
        }

        public List<BAG> GetBagByUserID(int id)
        {
            return db.BAGs.Where(n => n.MaTK == id).ToList();
        }
        public int GetFlimIDByDVDID(int id)
        {
            return (int)db.DVDs.Find(id).MaPhim;
        }
    }
}