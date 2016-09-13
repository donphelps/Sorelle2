using System.Web.Mvc;

namespace Sorelle.Web.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated)
				return new RedirectResult("/t", false);

			ViewBag.Title = "Sorelle";

			return View();
		}
	}
}