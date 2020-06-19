using System.Web.Mvc;

namespace LemonCat.Areas.ClientTicket
{
    public class ClientTicketAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ClientTicket";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ClientTicket_default",
                "ClientTicket/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}