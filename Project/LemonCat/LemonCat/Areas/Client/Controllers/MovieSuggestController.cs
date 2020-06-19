using LemonCat.Common;
using LemonCat.Models.DAO;
using LemonCat.Models.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LemonCat.Areas.Client.Controllers
{
    public class MovieSuggestController : Controller
    {
        // GET: Client/MovieSuggest
        public ActionResult Index()
        {
            if (Session[CommonConstaints.CLIENT_USER_SESSION] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<PHIM> result = new List<PHIM>();
            List<int> IdResult = new List<int>();
            if (SuggestDAO.Instance.CountDBSetByUserID((Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID) > 0)
            {
                HoiQuyTuyenTinh Model = new HoiQuyTuyenTinh();
                var List = SuggestDAO.Instance.GetModelByUserID((Session[CommonConstaints.CLIENT_USER_SESSION] as UserLogin).UserID);
                if (List != null)
                {
                    string[,] Input = new string[1000, 1000];
                    for (int i = 0; i < List.Count; i++)
                    {
                        Input[i, 1] = Math.Log((double)List[i].TheLoai) + "";
                        Input[i, 0] = (int)List[i].KetQua + "";
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        Random rnd = new Random();
                        int TestModelInput = rnd.Next(4, YearDataDAO.Instance.MovieCount());
                        IdResult.Add(Model.Result(Input, TestModelInput, List.Count));
                    }
                }
            }
            if (IdResult != null)
            {
                foreach (int item in IdResult)
                {
                    var subitem = MovieDAO.Instance.GetMovieByGenreID(item);
                    if (subitem != null)
                    {
                        result.Add(subitem);
                    }
                }
            }
            return View(result);
        }
    }
}