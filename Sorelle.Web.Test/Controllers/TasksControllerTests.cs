using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sorelle.Data.Repositories;
using Sorelle.Model;
using Sorelle.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.Mvc;

namespace Sorelle.Web.Controllers.Tests
{
	[TestClass]
	public class TasksControllerTests
	{
		private const string userid = "testUser";

		private Mock<ITaskRepository> taskRepository = null;
		private Mock<ITagRepository> tagRepository = null;
		private Mock<ControllerContext> controllerContext = null;
		private Mock<IPrincipal> principal = null;

		private TasksController controller;

		private List<Task> tasks = new List<Task>
		{
			new Task { Id = 1, Name = "1", UserId = userid, isGoal = true, isProject = true, Inboxed = true },
			new Task { Id = 2, Name = "2", UserId = userid, isGoal = true, isProject = true, Inboxed = true },
			new Task { Id = 3, Name = "3", UserId = userid, Inboxed = true },
			new Task { Id = 4, Name = "4", UserId = userid },
			new Task { Id = 5, Name = "5", UserId = userid }
		};

		private List<Tag> tags = new List<Tag>
		{
			new Tag { Id = 1, Name = "Pri", UserId = userid, Type = Model.Enums.TagType.Priority, Color = Model.Enums.TagColor.Red },
			new Tag { Id = 2, Name = "Sta", UserId = userid, Type = Model.Enums.TagType.Status, Color = Model.Enums.TagColor.Orange },
			new Tag { Id = 3, Name = "Pla", UserId = userid, Type = Model.Enums.TagType.Planning, Color = Model.Enums.TagColor.Yellow },
			new Tag { Id = 4, Name = "Con", UserId = userid, Type = Model.Enums.TagType.Constraint, Color = Model.Enums.TagColor.Green },
			new Tag { Id = 5, Name = "Oth", UserId = userid, Type = Model.Enums.TagType.Other, Color = Model.Enums.TagColor.Default }
		};

		[TestInitialize]
		public void Init()
		{
			tasks[2].ParentTask = tasks[0];
			tasks[4].ParentTask = tasks[0];
			tasks[3].ParentTask = tasks[1];

			tasks[0].PriorityTag = tags[0];
			tasks[0].StatusTag = tags[1];
			tasks[0].Tags.Add(tags[2]);
			tasks[0].Tags.Add(tags[3]);
			tasks[0].Tags.Add(tags[4]);

			controllerContext = new Mock<ControllerContext>();
			principal = new Mock<IPrincipal>();
			principal.SetupGet(x => x.Identity.Name).Returns(userid);
			controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
		}

		[TestMethod]
		public void Index()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.GetManyWithSpecialTags(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks.Where(w => w.ParentTask == null));

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Index() as ViewResult;
			var model = result.Model as IEnumerable<Task>;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Root Tasks", result.ViewBag.Title);
			Assert.IsNotNull(result.ViewBag.QuickAddInfo);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(2, model.Count());
		}

		[TestMethod]
		public void Details()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.GetDetailed(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Details(1) as ViewResult;
			var model = result.Model as Task;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Task Detail", result.ViewBag.Title);
			Assert.IsNotNull(result.ViewBag.QuickAddInfo);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(1, model.Id);
		}

		[TestMethod]
		public void Edit()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Edit(1, "returnUrl") as ViewResult;
			var model = result.Model as Task;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Edit Task", result.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(1, model.Id);
		}

		[TestMethod]
		public void EditPostModelInvalid()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			controller.ModelState.AddModelError("contains", "an error");

			// Act
			var result = controller.EditPost(tasks[0]) as ViewResult;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual("Edit Task", result.ViewBag.Title);
		}

		[TestMethod]
		public void EditPostModelValid()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.EditPost(tasks[0]) as RedirectResult;

			// Assert
			Assert.AreEqual("/t", result.Url);
		}

		[TestMethod]
		public void Goals()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.GetManyWithSpecialTags(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks.Where(w => w.isGoal));

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Goals() as ViewResult;
			var model = result.Model as IEnumerable<Task>;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Goal Tasks", result.ViewBag.Title);
			Assert.IsNotNull(result.ViewBag.QuickAddInfo);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(2, model.Count());
		}

		[TestMethod]
		public void InboxIsNotEmpty()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(r => r.GetNextInboxTask(It.IsAny<string>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Inbox() as ViewResult;
			var model = result.Model as Task;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Task Inbox", result.ViewBag.Title);
			Assert.IsNotNull(result.ViewBag.QuickAddInfo);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(1, model.Id);
		}

		[TestMethod]
		public void InboxIsEmpty()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(new Task { Id = 0 });

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Inbox() as ViewResult;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Inbox Empty", controller.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
		}

		[TestMethod]
		public void Inbox()
		{
		}

		[TestMethod]
		public void Projects()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.GetManyWithSpecialTags(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks.Where(w => w.isProject));

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Projects() as ViewResult;
			var model = result.Model as IEnumerable<Task>;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Project Tasks", result.ViewBag.Title);
			Assert.IsNotNull(result.ViewBag.QuickAddInfo);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(2, model.Count());
		}

		[TestMethod]
		public void QuickAdd()
		{
			// Arrange
			QuickAddViewModel qa = new QuickAddViewModel("qavm", 1, null, false, false, "/t");

			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();
			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			RedirectResult result = controller.QuickAdd(qa) as RedirectResult;

			// Assert
			Assert.AreEqual(true, tasks[0].Inboxed);
			Assert.AreEqual("/t", result.Url);
		}

		[TestMethod]
		public void ReviewCompleted()
		{
			// Arrange
			tasks[0].CompletedAt = DateTime.Now;
			tasks[1].CompletedAt = DateTime.Now;

			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.GetManyWithSpecialTags(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks.Where(w => w.CompletedAt != null));

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.ReviewCompleted() as ViewResult;
			var model = result.Model as IEnumerable<Task>;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Completed Tasks", result.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(2, model.Count());
		}

		[TestMethod]
		public void RemoveCompleted()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			tagRepository = new Mock<ITagRepository>();
			controller = new TasksController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			RedirectResult result = controller.RemoveComplete("/t") as RedirectResult;

			// Assert
			Assert.AreEqual("/t", result.Url);
		}
	}
}