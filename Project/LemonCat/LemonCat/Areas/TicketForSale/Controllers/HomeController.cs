using LemonCat.Common;
using LemonCat.Models.DAO;
using LemonCat.Models.EF;
using Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LemonCat.Areas.TicketForSale.Controllers
{
    public class HomeController : BaseController
    {
        // GET: TicketForSale/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BuyTicketIndex()
        {
            return View(TicketDAO.Instance.GetAllMovieInCinema());
        }
        [HttpGet]
        public ActionResult BuyTicket(int id)
        {
            //id->MovieID
            ViewBag.MovieID = id;
            ViewBag.TenPhim = MovieDAO.Instance.GetMovieNameByMovieID(id);
            ViewBag.ListNgayChieu = TicketDAO.Instance.GetNgayChieuByFlimID(id);
            ViewBag.LichTrongNgay = TicketDAO.Instance.GetSuatChieu();
            return View(TicketDAO.Instance.GetAllCinema());
        }
        [HttpPost]
        public ActionResult BuyTicket(FormCollection form)
        {
            return RedirectToAction("Booking", new RouteValueDictionary(
    new { controller = "Home", action = "Booking", IDRap = int.Parse(form["IDRap"]), IDPhim = int.Parse(form["IDPhim"]), DateTime = form["DateTime"] }));
        }
        [HttpGet]
        public ActionResult BookingList(int id)
        {
            //id->MovieID
            var result = TicketDAO.Instance.GetBookingByMovieID(id);
            List<ORDERSEAT> List = result.Item1;
            List<string> Name = result.Item2;
            ViewBag.List = List;
            ViewBag.Name = Name;
            ViewBag.MovieName = MovieDAO.Instance.GetMovieNameByMovieID(id);
            return View(List);
        }
        public ActionResult ShowBillTicket(string input)
        {
            int n;
            bool isNumeric = int.TryParse(input.Split(' ')[0], out n);
            if (isNumeric)
            {
                int result = TicketDAO.Instance.GetBillByQR(input);
                if (result != -1)
                {
                    return Json(new
                    {
                        Status = true,
                        ID = result
                    });
                }
            }
            return RedirectToAction("Index", "Error");
        }
        [HttpGet]
        public ActionResult Booking(int IDRap, int IDPhim, string DateTime)
        {
            ViewBag.TenNhanVien = UserDAO.Instance.GetNameByID((Session[CommonConstaints.TICKETSTAFF_USER_SESSION] as UserLogin).UserID);
            ViewBag.IDRap = IDRap;
            ViewBag.IDPhim = IDPhim;
            ViewBag.DateTime = DateTime;
            List<string> A = new List<string>();
            List<string> B = new List<string>();
            List<string> C = new List<string>();
            List<string> D = new List<string>();
            List<string> E = new List<string>();
            List<string> F = new List<string>();
            List<string> G = new List<string>();
            List<string> H = new List<string>();
            List<string> I = new List<string>();
            List<string> J = new List<string>();
            List<string> K = new List<string>();
            List<string> L = new List<string>();
            List<string> M = new List<string>();
            List<string> N = new List<string>();
            List<string> O = new List<string>();
            List<string> P = new List<string>();
            List<string> Q = new List<string>();
            List<string> R = new List<string>();
            string EmptySeat = TicketDAO.Instance.GetEmptySeat(IDRap, IDPhim, DateTime);
            char c;
            for (c = 'A'; c <= 'R'; ++c)
            {
                for (int i = 1; i <= 18; i++)
                {
                    if (!EmptySeat.Contains(c + i.ToString()))
                    {
                        switch (c)
                        {
                            case 'A':
                                A.Add(c + i.ToString());
                                break;
                            case 'B':
                                B.Add(c + i.ToString());
                                break;
                            case 'C':
                                C.Add(c + i.ToString());
                                break;
                            case 'D':
                                D.Add(c + i.ToString());
                                break;
                            case 'E':
                                E.Add(c + i.ToString());
                                break;
                            case 'F':
                                F.Add(c + i.ToString());
                                break;
                            case 'G':
                                G.Add(c + i.ToString());
                                break;
                            case 'H':
                                H.Add(c + i.ToString());
                                break;
                            case 'I':
                                I.Add(c + i.ToString());
                                break;
                            case 'J':
                                J.Add(c + i.ToString());
                                break;
                            case 'K':
                                K.Add(c + i.ToString());
                                break;
                            case 'L':
                                L.Add(c + i.ToString());
                                break;
                            case 'M':
                                M.Add(c + i.ToString());
                                break;
                            case 'N':
                                N.Add(c + i.ToString());
                                break;
                            case 'O':
                                O.Add(c + i.ToString());
                                break;
                            case 'P':
                                P.Add(c + i.ToString());
                                break;
                            case 'Q':
                                Q.Add(c + i.ToString());
                                break;
                            case 'R':
                                R.Add(c + i.ToString());
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            ViewBag.A = A;
            ViewBag.B = B;
            ViewBag.C = C;
            ViewBag.D = D;
            ViewBag.E = E;
            ViewBag.F = F;
            ViewBag.G = G;
            ViewBag.H = H;
            ViewBag.I = I;
            ViewBag.J = J;
            ViewBag.K = K;
            ViewBag.L = L;
            ViewBag.M = M;
            ViewBag.N = N;
            ViewBag.O = O;
            ViewBag.P = P;
            ViewBag.Q = Q;
            ViewBag.R = R;
            return View();
        }
        [HttpPost]
        public ActionResult Booking(int IDRap, int IDPhim, string DateTime, string SeatList)
        {
            if (SeatList != "")
            {
                int account = (Session[CommonConstaints.TICKETSTAFF_USER_SESSION] as UserLogin).UserID;
                List<string> Seat = SeatList.Split(' ').ToList();
                int result = TicketDAO.Instance.CreateBill(account, Seat, DateTime, IDRap, IDPhim, Calendar.GetNow());
                if (result > 0)
                {
                    return Json(new
                    {
                        Status = true,
                        ID = result
                    });
                }
            }
            return RedirectToAction("Index", "Error");
        }
        [HttpGet]
        public ActionResult Bill(int id)
        {
            //id->BillID
            var result = TicketDAO.Instance.GetBillByBillID(id);
            List<string> Seat = result.DanhSachGheDat.Split(' ').ToList();
            Seat.RemoveAt(Seat.Count - 1);
            ViewBag.Seat = Seat;
            var info = TicketDAO.Instance.GetDateActiveByBillID(id);
            ViewBag.Date = info.Item1;
            ViewBag.Time = info.Item2;
            ViewBag.CinemaName = info.Item3;
            ViewBag.MovieName = info.Item4;
            return View(result);
        }
    }
}