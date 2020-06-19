using LemonCat.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LemonCat.Areas.TicketForSale.Controllers
{
    public class BaseController : Controller
    {
        // GET: TicketForSale/Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = Session[CommonConstaints.TICKETSTAFF_USER_SESSION] as UserLogin;
            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Login", action = "Index", Area = "TicketForSale" }));

            }
            base.OnActionExecuting(filterContext);
        }
    }
}