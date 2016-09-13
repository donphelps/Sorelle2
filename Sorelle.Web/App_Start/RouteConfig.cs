using System.Web.Mvc;
using System.Web.Routing;

namespace Sorelle.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapMvcAttributeRoutes();

			routes.MapRoute(
				name: "AjaxlessAPITaskTagId",
				url: "api/task/{id}/{action}/{tagid}",
				defaults: new { controller = "Ajaxless", tagid = UrlParameter.Optional });

			routes.MapRoute(
				name: "AjaxlessAPITasksTagId",
				url: "api/tasks/{id}/{action}/{tagid}",
				defaults: new { controller = "Ajaxless", tagid = UrlParameter.Optional });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}