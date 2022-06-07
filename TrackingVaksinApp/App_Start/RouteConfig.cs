using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TrackingVaksinApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Autentication",
                url: "Auth/{controller}/{action}/{id}",
                defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Produsen", 
                url: "Produsen/{controller}/{action}/{id}", 
                defaults: new { controller = "HomeProdusen", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "RS", 
                url: "RS/{controller}/{action}/{id}", 
                defaults: new { controller = "HomeRS", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{path}/{controller}/{action}/{id}",
                defaults: new {path="BPOM",controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
