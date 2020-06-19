using LemonCat.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LemonCat.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = Session[CommonConstaints.USER_SESSION] as UserLogin;
            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Login", action = "Index", Area = "Admin" }));

            }
            base.OnActionExecuting(filterContext);
        }
        protected void SetAlert(string mess, string type)
        {
            TempData["AlertMessage"] = mess;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
    }
}