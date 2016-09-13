using Microsoft.AspNet.Identity;
using Sorelle.Data.Repositories;
using Sorelle.Services;
using System.Web.Mvc;

namespace Sorelle.Web.Controllers
{
	[Authorize]
	public class AjaxlessAPIController : Controller
	{
		private TagService tagService;
		private TaskService taskService;

		public AjaxlessAPIController(ITaskRepository taskRepository, ITagRepository tagRepository)
		{
			taskService = new TaskService(taskRepository, tagRepository);
			tagService = new TagService(tagRepository, taskRepository);
		}

		[Route("api/task/{id:int}/attachtag/{tagid:int}")]
		public ActionResult AttachTag(int id, int tagid, string returnUrl)
		{
			if (!taskService.AttachTag(id, tagid, User.Identity.GetUserId()))
				return new HttpStatusCodeResult(500);

			if (string.IsNullOrEmpty(returnUrl))
				return new RedirectResult("/t", false);

			return new RedirectResult(returnUrl, false);
		}

		[Route("api/task/{id:int}/complete")]
		public ActionResult Complete(int id, string returnUrl)
		{
			if (!taskService.Complete(id, User.Identity.GetUserId()))
				return new HttpStatusCodeResult(500);

			if (string.IsNullOrEmpty(returnUrl))
				return new RedirectResult("/t", false);

			return new RedirectResult(returnUrl, false);
		}

		[Route("api/task/{id:int}/detachtag/{tagid:int}")]
		public ActionResult DetachTag(int id, int tagid, string returnUrl)
		{
			if (!taskService.DetachTag(id, tagid, User.Identity.GetUserId()))
				return new HttpStatusCodeResult(500);

			if (string.IsNullOrEmpty(returnUrl))
				return new RedirectResult("/t", false);

			return new RedirectResult(returnUrl, false);
		}

		[Route("api/task/inbox/{id:int}/remove")]
		public ActionResult InboxRemove(int id)
		{
			if (!taskService.InboxRemove(id, User.Identity.GetUserId()))
				return new HttpStatusCodeResult(500);

			return new RedirectResult("/t/inbox", false);
		}

		[Route("api/task/{id:int}/togglegoal")]
		public ActionResult ToggleGoal(int id, string returnUrl)
		{
			if (!taskService.ToggleGoal(id, User.Identity.GetUserId()))
				return new HttpStatusCodeResult(500);

			if (string.IsNullOrEmpty(returnUrl))
				return new RedirectResult("/t", false);

			return new RedirectResult(returnUrl, false);
		}

		[Route("api/task/{id:int}/togglepin")]
		public ActionResult TogglePin(int id, string returnUrl)
		{
			if (!taskService.TogglePin(id, User.Identity.GetUserId()))
				return new HttpStatusCodeResult(500);

			if (string.IsNullOrEmpty(returnUrl))
				return new RedirectResult("/t", false);

			return new RedirectResult(returnUrl, false);
		}

		[Route("api/task/{id:int}/toggleproject")]
		public ActionResult ToggleProject(int id, string returnUrl)
		{
			if (!taskService.ToggleProject(id, User.Identity.GetUserId()))
				return new HttpStatusCodeResult(500);

			if (string.IsNullOrEmpty(returnUrl))
				return new RedirectResult("/t", false);

			return new RedirectResult(returnUrl, false);
		}

		[Route("api/task/{id:int}/uncomplete")]
		public ActionResult Uncomplete(int id, string returnUrl)
		{
			if (!taskService.Uncomplete(id, User.Identity.GetUserId()))
				return new HttpStatusCodeResult(500);

			if (string.IsNullOrEmpty(returnUrl))
				return new RedirectResult("/t", false);

			return new RedirectResult(returnUrl, false);
		}
	}
}