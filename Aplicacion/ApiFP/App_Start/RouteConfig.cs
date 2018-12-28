using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiFP.App_Start
{

    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            // By default route the user to the Help area if accessing the base URI.
            routes.MapRoute(
                "Help Area",
                "help",
                new { controller = "Help", action = "Index" })
                .DataTokens = new RouteValueDictionary(new { area = "HelpPage" });
        }
    }

}