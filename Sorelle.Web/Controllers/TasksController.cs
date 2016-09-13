using Microsoft.AspNet.Identity;
using Sorelle.Data.Repositories;
using Sorelle.Model;
using Sorelle.Services;
using Sorelle.Web.Models;
using System.Web.Mvc;

namespace Sorelle.Web.Controllers
{
	[Authorize]
	public class TasksController : Controller
	{
		private TaskService taskService;
		private TagService tagService;

		public TasksController(ITaskRepository taskRepository, ITagRepository tagRepository)
		{
			taskService = new TaskService(taskRepository, tagRepository);
			tagService = new TagService(tagRepository, taskRepository);
		}

		[Route("t")]
		public ActionResult Index()
		{
			ViewBag.QuickAddInfo = new QuickAddViewModel("", null, null, false, false, "/t");
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Root Tasks";
			return View("List", taskService.GetAllRoot(User.Identity.GetUserId()));
		}

		[Route("t/{id:int}")]
		public ActionResult Details(int id)
		{
			Task task = taskService.GetDetailed(id, User.Identity.GetUserId());
			if (task == null)
				return HttpNotFound();

			TagList tagList = tagService.GetAll(User.Identity.GetUserId());

			ViewBag.TagList = tagList;
			ViewBag.SelectTags = new TagSelectList(task, tagList);
			ViewBag.QuickAddInfo = new QuickAddViewModel("", task.Id, null, false, false, $"/t/{task.Id}");
			ViewBag.Title = "Task Detail";
			return View(task);
		}

		[Route("t/edit/{id:int}")]
		public ActionResult Edit(int id, string returnUrl)
		{
			Task task = taskService.Get(id, User.Identity.GetUserId());
			if (task == null)
				return HttpNotFound();

			TempData["returnUrl"] = returnUrl;

			ViewBag.PriorityTagId = new SelectList(tagService.SelectListPriorityTags(User.Identity.GetUserId()), "Id", "UserId", task.PriorityTag?.Id);
			ViewBag.StatusTagId = new SelectList(tagService.SelectListStatusTags(User.Identity.GetUserId()), "Id", "UserId", task.StatusTag?.Id);

			task.UserId = "edit";
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Edit Task";
			return View(task);
		}

		[HttpPost]
		[Route("t/edit/{id:int}")]
		[ValidateAntiForgeryToken]
		public ActionResult EditPost(Task task)
		{
			if (ModelState.IsValid)
			{
				if (!taskService.Edit(task, User.Identity.GetUserId()))
					return new HttpStatusCodeResult(500);

				string returnUrl = TempData["returnUrl"]?.ToString();
				if (string.IsNullOrEmpty(returnUrl))
					return new RedirectResult("/t", false);

				return new RedirectResult(returnUrl, false);
			}

			ViewBag.PriorityTagId = new SelectList(tagService.SelectListPriorityTags(User.Identity.GetUserId()), "Id", "UserId", task.PriorityTag?.Id);
			ViewBag.StatusTagId = new SelectList(tagService.SelectListStatusTags(User.Identity.GetUserId()), "Id", "UserId", task.StatusTag?.Id);

			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Edit Task";
			return View(task);
		}

		[Route("t/goals")]
		public ActionResult Goals()
		{
			ViewBag.QuickAddInfo = new QuickAddViewModel("", null, null, false, true, "/t/goals");
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Goal Tasks";
			return View("List", taskService.GetAllGoals(User.Identity.GetUserId()));
		}

		[Route("t/inbox")]
		public ActionResult Inbox()
		{
			Task task = taskService.GetNextInboxTask(User.Identity.GetUserId());

			if (task == null)
			{
				ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
				ViewBag.Title = "Inbox Empty";
				return View("InboxEmpty");
			}

			var tagList = tagService.GetAll(User.Identity.GetUserId());

			ViewBag.TagList = tagList;
			ViewBag.SelectTags = new TagSelectList(task, tagList);
			ViewBag.QuickAddInfo = new QuickAddViewModel("", task.Id, null, false, false, $"/t/inbox/{task.Id}");
			ViewBag.Title = "Task Inbox";
			return View(task);
		}

		[Route("t/projects")]
		public ActionResult Projects()
		{
			ViewBag.QuickAddInfo = new QuickAddViewModel("", null, null, true, false, "/t/projects");
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Project Tasks";
			return View("List", taskService.GetAllProjects(User.Identity.GetUserId()));
		}

		[HttpPost]
		[Route("t/quickadd")]
		[ValidateAntiForgeryToken]
		public ActionResult QuickAdd(QuickAddViewModel quickadd)
		{
			if (ModelState.IsValid)
			{
				Task task = new Task(quickadd.Name, User.Identity.GetUserId());

				if (quickadd.ParentId.HasValue)
				{
					if (!taskService.QuickAddWithParent(task, quickadd.ParentId.Value, User.Identity.GetUserId()))
						return new HttpStatusCodeResult(500);
				}
				else if (quickadd.TagId.HasValue)
				{
					if (!taskService.QuickAddWithTag(task, quickadd.TagId.Value, User.Identity.GetUserId()))
						return new HttpStatusCodeResult(500);
				}
				else if (quickadd.Goal)
					taskService.QuickAddAsGoal(task, User.Identity.GetUserId());
				else if (quickadd.Project)
					taskService.QuickAddAsProject(task, User.Identity.GetUserId());
				else
					taskService.Create(task, User.Identity.GetUserId());
			}

			if (string.IsNullOrEmpty(quickadd.ReturnUrl))
				return new RedirectResult("/t", false);

			return new RedirectResult(quickadd.ReturnUrl, false);
		}

		[Route("t/completed")]
		public ActionResult ReviewCompleted()
		{
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Completed Tasks";
			return View(taskService.GetAllRecentlyCompleted(User.Identity.GetUserId()));
		}

		[Route("t/removecomplete")]
		public ActionResult RemoveComplete(string returnUrl)
		{
			if (string.IsNullOrEmpty(returnUrl))
				returnUrl = "/t";

			taskService.RemoveAllComplete(User.Identity.GetUserId());

			return new RedirectResult(returnUrl, false);
		}
	}
}