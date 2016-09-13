using Microsoft.AspNet.Identity;
using Sorelle.Data.Repositories;
using Sorelle.Model;
using Sorelle.Model.Enums;
using Sorelle.Services;
using Sorelle.Web.Models;
using System.Web.Mvc;

namespace Sorelle.Web.Controllers
{
	[Authorize]
	public class TagsController : Controller
	{
		private TaskService taskService;
		private TagService tagService;

		public TagsController(ITagRepository tagRepository, ITaskRepository taskRepository)
		{
			tagService = new TagService(tagRepository, taskRepository);
			taskService = new TaskService(taskRepository, tagRepository);
		}

		[Route("tag/delete/{id:int}")]
		public ActionResult Delete(int id, string returnUrl)
		{
			Tag tag = tagService.Get(id, User.Identity.GetUserId());
			if (tag == null)
				return HttpNotFound();

			TempData["returnUrl"] = returnUrl;

			tag.UserId = "delete";
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Confirm Tag Delete";
			return View(tag);
		}

		[HttpPost]
		[Route("tag/delete/{id:int}")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			tagService.Delete(id, User.Identity.GetUserId());

			string returnUrl = TempData["returnUrl"]?.ToString();
			if (string.IsNullOrEmpty(returnUrl))
				return new RedirectResult("/tags", false);

			return new RedirectResult(returnUrl, false);
		}

		[Route("tags")]
		public ActionResult Index()
		{
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Tags";
			return View();
		}

		[Route("tag/{id:int}")]
		public ActionResult Details(int id)
		{
			Tag tag = tagService.GetWithTasks(id, User.Identity.GetUserId());

			if (tag == null)
				return HttpNotFound();

			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.QuickAddInfo = new QuickAddViewModel("", null, tag.Id, false, false, $"{tag.Id}");
			ViewBag.Title = "Tag Details";
			return View(tag);
		}

		[Route("tag/edit/{id:int}")]
		public ActionResult Edit(int id, string returnUrl)
		{
			Tag tag = tagService.Get(id, User.Identity.GetUserId());
			if (tag == null)
				return HttpNotFound();

			TempData["returnUrl"] = returnUrl;

			tag.UserId = "edit";
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Edit Tag";
			return View(tag);
		}

		[HttpPost]
		[Route("tag/edit/{id:int}")]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Tag tag)
		{
			if (ModelState.IsValid)
			{
				tag.UserId = User.Identity.GetUserId();
				if (!tagService.Edit(tag, User.Identity.GetUserId()))
					return new HttpStatusCodeResult(500);

				string returnUrl = TempData["returnUrl"]?.ToString();
				if (string.IsNullOrEmpty(returnUrl))
					return new RedirectResult("/tags", false);

				return new RedirectResult(returnUrl, false);
			}

			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "Edit Tag";
			return View(tag);
		}

		[Route("tag/new/{tagType?}")]
		public ActionResult New(string tagType, string returnUrl)
		{
			Tag tag = new Tag();

			if (tagType == null)
				tagType = "";

			switch (tagType.ToLower())
			{
				case "priority":
					tag.Type = TagType.Priority;
					break;

				case "status":
					tag.Type = TagType.Status;
					break;

				case "planning":
					tag.Type = TagType.Planning;
					break;

				case "constraint":
					tag.Type = TagType.Constraint;
					break;

				default:
					tag.Type = TagType.Other;
					break;
			}

			TempData["returnUrl"] = returnUrl;

			tag.UserId = "new";
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "New Tag";
			return View(tag);
		}

		[HttpPost]
		[Route("tag/new/{tagType?}")]
		[ValidateAntiForgeryToken]
		public ActionResult New(Tag tag)
		{
			if (ModelState.IsValid)
			{
				tagService.Create(tag, User.Identity.GetUserId());

				string returnUrl = TempData["returnUrl"]?.ToString();
				if (string.IsNullOrEmpty(returnUrl))
					return new RedirectResult("/tags", false);

				return new RedirectResult(returnUrl, false);
			}

			tag.UserId = "new";
			ViewBag.TagList = tagService.GetAll(User.Identity.GetUserId());
			ViewBag.Title = "New Tag";
			return View(tag);
		}
	}
}