using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sorelle.Data.Repositories;
using Sorelle.Model;
using Sorelle.Model.Enums;
using Sorelle.Web.Controllers;
using Sorelle.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.Mvc;

namespace Sorelle.Web.Test.Controllers
{
	[TestClass]
	public class TagsControllerTests
	{
		private const string userid = "testUser";

		private Mock<ITaskRepository> taskRepository = null;
		private Mock<ITagRepository> tagRepository = null;
		private Mock<ControllerContext> controllerContext = null;
		private Mock<IPrincipal> principal = null;

		private TagsController controller;

		private List<Task> tasks = new List<Task>
		{
			new Task { Id = 1, Name = "1", UserId = userid, isGoal = true, isProject = true },
			new Task { Id = 2, Name = "2", UserId = userid, isGoal = true, isProject = true },
			new Task { Id = 3, Name = "3", UserId = userid },
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
		public void Delete()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);
			tagRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Delete(1, "returnUrl") as ViewResult;
			var model = result.Model as Tag;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Confirm Tag Delete", result.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(1, model.Id);
		}

		[TestMethod]
		public void DeleteConfirmed()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.DeleteConfirmed(1) as RedirectResult;

			// Assert
			Assert.AreEqual("/tags", result.Url);
		}

		[TestMethod]
		public void Index()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Index() as ViewResult;
			var taglisting = result.ViewBag.TagList as TagList;

			// Assert
			Assert.AreEqual("Tags", result.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
		}

		[TestMethod]
		public void Details()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);
			tagRepository
				.Setup(s => s.GetWithTasks(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Details(1) as ViewResult;
			var taglisting = result.ViewBag.TagList as TagList;
			var model = result.Model as Tag;

			// Assert
			Assert.AreEqual("Tag Details", result.ViewBag.Title);
			Assert.IsNotNull(result.ViewBag.QuickAddInfo as QuickAddViewModel);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(1, model.Id);
		}

		[TestMethod]
		public void Edit()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);
			tagRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Edit(1, "returnUrl") as ViewResult;
			var taglisting = result.ViewBag.TagList as TagList;
			var model = result.Model as Tag;

			// Assert
			Assert.AreEqual("Edit Tag", result.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(1, model.Id);
		}

		[TestMethod]
		public void EditPostModelNotValid()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);
			tagRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			controller.ModelState.AddModelError("contains", "an error");

			// Act
			var result = controller.Edit(tags[0]) as ViewResult;
			var taglisting = result.ViewBag.TagList as TagList;
			var model = result.Model as Tag;

			// Assert
			Assert.AreEqual("Edit Tag", result.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(1, model.Id);
		}

		[TestMethod]
		public void EditPostModelValid()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Edit(tags[0]) as RedirectResult;

			// Assert
			Assert.AreEqual("/tags", result.Url);
		}

		[TestMethod]
		public void New()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);
			tagRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.New("priority", "returnUrl") as ViewResult;
			var taglisting = result.ViewBag.TagList as TagList;
			var model = result.Model as Tag;

			// Assert
			Assert.AreEqual("New Tag", result.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(TagType.Priority, model.Type);
		}

		[TestMethod]
		public void NewPostModelNotValid()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags);
			tagRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.New("priority", "returnUrl") as ViewResult;
			var taglisting = result.ViewBag.TagList as TagList;
			var model = result.Model as Tag;

			// Assert
			Assert.AreEqual("New Tag", result.ViewBag.Title);
			Assert.AreEqual(1, taglisting.Priority.Count);
			Assert.AreEqual(TagType.Priority, model.Type);
		}

		[TestMethod]
		public void NewPostModelValid()
		{
			// Arrange
			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			taskRepository = new Mock<ITaskRepository>();

			controller = new TagsController(tagRepository.Object, taskRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.New(tags[0]) as RedirectResult;

			// Assert
			Assert.AreEqual("/tags", result.Url);
		}
	}
}