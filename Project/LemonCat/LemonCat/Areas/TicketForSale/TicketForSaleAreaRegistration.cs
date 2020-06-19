using System.Web.Mvc;

namespace LemonCat.Areas.TicketForSale
{
    public class TicketForSaleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TicketForSale";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TicketForSale_default",
                "TicketForSale/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}