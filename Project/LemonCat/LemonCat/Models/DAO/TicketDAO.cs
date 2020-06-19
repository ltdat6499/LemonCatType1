using LemonCat.Common;
using LemonCat.Models.EF;
using Model.DAO;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace LemonCat.Models.DAO
{
    public class TicketDAO
    {
        private static TicketDAO instance;
        LemonCatEntities db = null;
        public TicketDAO()
        {
            db = new LemonCatEntities();
        }

        public List<PHIM> GetAllMovieInCinema()
        {
            var list = db.HanChieux.ToList();
            List<PHIM> result = new List<PHIM>();
            foreach (var item in list)
            {
                if (Calendar.SubDate(Calendar.GetNow(), item.EndDate) >= 0)
                {
                    result.Add(MovieDAO.Instance.GetMovie((int)item.MaPhim));
                }
            }
            return result;
        }

        public List<PHIM> GetMovieNowShowingOnCinema()
        {
            var list = db.HanChieux.ToList();
            List<PHIM> result = new List<PHIM>();
            foreach (var item in list)
            {
                if (Calendar.SubDate(DateTime.Now.ToString("dd/MM/yyyy").Replace("/", "-"), item.EndDate) > 0)
                {
                    result.Add(MovieDAO.Instance.GetMovie((int)item.MaPhim));
                }
            }
            return result;
        }
        public PHIM GetNameMovieByIDSuatChieu(int id)
        {
            return MovieDAO.Instance.GetMovie((int)db.SUATCHIEUx.Find(id).IDPhim);
        }
        public static TicketDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new TicketDAO();
                return TicketDAO.instance;
            }
            private set
            {
                TicketDAO.instance = value;
            }
        }
        string FullSeat = "A1 A2 A3 A4 A5 A6 A7 A8 A9 A10 A11 A12 A13 A14 A15 A16 A17 A18 B1 B2 B3 B4 B5 B6 B7 B8 B9 B10 B11 B12 B13 B14 B15 B16 B17 B18 C1 C2 C3 C4 C5 C6 C7 C8 C9 C10 C11 C12 C13 C14 C15 C16 C17 C18 D1 D2 D3 D4 D5 D6 D7 D8 D9 D10 D11 D12 D13 D14 D15 D16 D17 D18 E1 E2 E3 E4 E5 E6 E7 E8 E9 E10 E11 E12 E13 E14 E15 E16 E17 E18 F1 F2 F3 F4 F5 F6 F7 F8 F9 F10 F11 F12 F13 F14 F15 F16 F17 F18 G1 G2 G3 G4 G5 G6 G7 G8 G9 G10 G11 G12 G13 G14 G15 G16 G17 G18 H1 H2 H3 H4 H5 H6 H7 H8 H9 H10 H11 H12 H13 H14 H15 H16 H17 H18 I1 I2 I3 I4 I5 I6 I7 I8 I9 I10 I11 I12 I13 I14 I15 I16 I17 I18 J1 J2 J3 J4 J5 J6 J7 J8 J9 J10 J11 J12 J13 J14 J15 J16 J17 J18 K1 K2 K3 K4 K5 K6 K7 K8 K9 K10 K11 K12 K13 K14 K15 K16 K17 K18 L1 L2 L3 L4 L5 L6 L7 L8 L9 L10 L11 L12 L13 L14 L15 L16 L17 L18 M1 M2 M3 M4 M5 M6 M7 M8 M9 M10 M11 M12 M13 M14 M15 M16 M17 M18 N1 N2 N3 N4 N5 N6 N7 N8 N9 N10 N11 N12 N13 N14 N15 N16 N17 N18 O1 O2 O3 O4 O5 O6 O7 O8 O9 O10 O11 O12 O13 O14 O15 O16 O17 O18 P1 P2 P3 P4 P5 P6 P7 P8 P9 P10 P11 P12 P13 P14 P15 P16 P17 P18 Q1 Q2 Q3 Q4 Q5 Q6 Q7 Q8 Q9 Q10 Q11 Q12 Q13 Q14 Q15 Q16 Q17 Q18 R1 R2 R3 R4 R5 R6 R7 R8 R9 R10 R11 R12 R13 R14 R15 R16 R17 R18 ";

        public int GetBillByQR(string input)
        {
            int id = int.Parse(input.Split(' ')[0]);
            if (db.ORDERSEATs.Find(id) == null)
            {
                return -1;
            }
            else
            {
                return id;
            }
        }

        public int GetBoxOfficeByMovieID(int maPhim)
        {
            int result = 0;
            var list = db.SUATCHIEUx.Where(n => n.IDPhim == maPhim);
            foreach (var item in list)
            {
                var billList = db.ORDERSEATs.Where(n => n.IDSuatChieu == item.ID);
                foreach (var subitem in billList)
                {
                    result += (int)subitem.TongTien;
                }
            }
            return result;
        }
        public string GetNameCinemaByID(int id)
        {
            return db.RAPPHIMs.Find(id).TenRap;
        }
        public Tuple<List<ORDERSEAT>, List<string>> GetBookingByMovieID(int maPhim)
        {
            List<ORDERSEAT> result = new List<ORDERSEAT>();
            List<string> name = new List<string>();
            var list = db.SUATCHIEUx.Where(n => n.IDPhim == maPhim);
            foreach (var item in list)
            {
                var billList = db.ORDERSEATs.Where(n => n.IDSuatChieu == item.ID);
                foreach (var subitem in billList)
                {
                    result.Add(subitem);
                    name.Add(GetNameCinemaByID((int)item.IDRap));
                }
            }
            return new Tuple<List<ORDERSEAT>, List<string>>(result, name);
        }
        public Tuple<List<ORDERSEAT>, List<string>> GetBookingByUserID(int account)
        {
            List<ORDERSEAT> result = new List<ORDERSEAT>();
            List<string> name = new List<string>();
            var list = db.SUATCHIEUx.ToList();
            foreach (var item in list)
            {
                var billList = db.ORDERSEATs.Where(n => n.IDSuatChieu == item.ID && n.MaTK == account);
                foreach (var subitem in billList)
                {
                    result.Add(subitem);
                    name.Add(GetNameCinemaByID((int)item.IDRap));
                }
            }
            return new Tuple<List<ORDERSEAT>, List<string>>(result, name);
        }

        public List<string> GetNgayChieuByFlimID(int id)
        {
            int count = db.HanChieux.Where(n => n.MaPhim == id).Count();
            if (count > 0)
            {
                string HanChieu = db.HanChieux.Where(n => n.MaPhim == id).FirstOrDefault().EndDate;
                List<string> result = Calendar.GetDatesBetween(HanChieu);
                return result;
            }
            else
            {
                return null;
            }
        }

        int[] SuatChieu = { 8, 9, 10, 11, 13, 15, 17, 19, 20, 21, 22 };
        int Price = 10;
        public string GetEmptySeat(int IDRap, int IDPhim, string DateTime)
        {
            int count = db.SUATCHIEUx.Where(n => n.IDRap == IDRap && n.IDPhim == IDPhim && n.Time == DateTime).Count();
            if (count > 0)
            {
                return db.SUATCHIEUx.Where(n => n.IDRap == IDRap && n.IDPhim == IDPhim && n.Time == DateTime).FirstOrDefault().DanhSachGheTrong;
            }
            else
            {
                SUATCHIEU result = new SUATCHIEU();
                result.DanhSachGheTrong = FullSeat;
                result.IDPhim = IDPhim;
                result.IDRap = IDRap;
                result.Time = DateTime;
                result.Price = Price;
                db.SUATCHIEUx.Add(result);
                db.SaveChanges();
                return result.DanhSachGheTrong;
            }
        }
        public List<int> GetSuatChieu()
        {
            return SuatChieu.ToList();
        }
        public void SetDateOnTheaterByMovieID(int id, string sdate, string edate)
        {
            int count = db.HanChieux.Where(n => n.MaPhim == id).Count();
            if (count > 0)
            {
                var result = db.HanChieux.Where(n => n.MaPhim == id).FirstOrDefault();
                result.StartDate = sdate;
                result.EndDate = edate;
                db.SaveChanges();
            }
            else
            {
                HanChieu result = new HanChieu();
                result.MaPhim = id;
                result.StartDate = sdate;
                result.EndDate = edate;
                db.HanChieux.Add(result);
                db.SaveChanges();
            }
        }
        public int CreateBill(int Account, List<string> Seat, string DateTime, int IDRap, int IDPhim, string DateNow)
        {
            Seat.RemoveAt(Seat.Count - 1);
            if (Order(Seat, DateTime, IDRap, IDPhim))
            {
                var SuatChieu = db.SUATCHIEUx.Where(n => n.IDRap == IDRap && n.IDPhim == IDPhim && n.Time == DateTime).FirstOrDefault();
                ORDERSEAT result = new ORDERSEAT();
                result.MaTK = Account;
                result.IDSuatChieu = SuatChieu.ID;
                result.DanhSachGheDat = "";
                foreach (string item in Seat)
                {
                    result.DanhSachGheDat += item + " ";
                }
                result.TongTien = SuatChieu.Price * Seat.Count();
                result.TapDoan = "LemonCat";
                result.DiaChiTapDoan = "SPKT Vo Van Ngan Road";
                result.TenKhach = UserDAO.Instance.GetNameByID(Account);
                result.Sdt = UserDAO.Instance.GetPhoneByID(Account);
                result.Email = UserDAO.Instance.GetEmailByID(Account);
                result.NgayTaoHoaDon = DateNow;
                result.Price = Price;
                db.ORDERSEATs.Add(result);
                db.SaveChanges();
                string QR = result.ID + " " + Account + " " + result.DanhSachGheDat + DateTime + " " + IDRap + " " + IDPhim;
                result.QR = CreateQR(QR);
                db.SaveChanges();
                return result.ID;
            }
            return -1;
        }
        private string CreateQR(string input)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(input, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            using (Bitmap bitMap = qrCode.GetGraphic(20))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        private bool IsEmptySeats(List<string> Seat, string OrderSeat)
        {
            foreach (string item in Seat)
            {
                if (!OrderSeat.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
        private bool Order(List<string> Seat, string DateTime, int IDRap, int IDPhim)
        {
            int count = db.SUATCHIEUx.Where(n => n.IDRap == IDRap && n.IDPhim == IDPhim && n.Time == DateTime).Count();
            if (count == 1)
            {
                //Check Empty Seat
                var temp = db.SUATCHIEUx.Where(n => n.IDRap == IDRap && n.IDPhim == IDPhim && n.Time == DateTime).FirstOrDefault();
                string EmptySeatList = temp.DanhSachGheTrong;
                if (Seat != null)
                {
                    if (IsEmptySeats(Seat, EmptySeatList))
                    {
                        foreach (string item in Seat)
                        {
                            EmptySeatList = EmptySeatList.Replace(item + " ", "");
                        }
                        temp.DanhSachGheTrong = EmptySeatList;
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (count > 1) 
                return false;
            else
            {
                SUATCHIEU result = new SUATCHIEU();
                result.DanhSachGheTrong = FullSeat;
                result.IDPhim = IDPhim;
                result.IDRap = IDRap;
                result.Price = 10;
                result.Time = DateTime;
                foreach (string item in Seat)
                {
                    result.DanhSachGheTrong = result.DanhSachGheTrong.Replace(item + " ", "");
                }
                db.SUATCHIEUx.Add(result);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public Tuple<string, string , string, string> GetDateActiveByBillID(int id)
        {
            var bill = db.ORDERSEATs.Find(id);
            string Date = db.SUATCHIEUx.Find(bill.IDSuatChieu).Time.Split(' ')[0];
            string Time = db.SUATCHIEUx.Find(bill.IDSuatChieu).Time.Split(' ')[1];
            string CinemaName = db.RAPPHIMs.Find(db.SUATCHIEUx.Find(bill.IDSuatChieu).IDRap).TenRap;
            string MovieName = MovieDAO.Instance.GetMovieNameByMovieID((int)db.SUATCHIEUx.Find(bill.IDSuatChieu).IDPhim);
            return new Tuple<string, string, string, string>(Date, Time, CinemaName, MovieName);
        }

        public ORDERSEAT GetBillByBillID(int id)
        {
            return db.ORDERSEATs.Find(id);
        }
        public List<RAPPHIM> GetAllCinema()
        {
            return db.RAPPHIMs.ToList();
        }
    }
}