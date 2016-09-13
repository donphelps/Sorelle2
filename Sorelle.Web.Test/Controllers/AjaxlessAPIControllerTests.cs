using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sorelle.Data.Repositories;
using Sorelle.Model;
using Sorelle.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Web.Mvc;

namespace Sorelle.Web.Test.Controllers
{
	[TestClass]
	public class AjaxlessAPIControllerTests
	{
		private const string userid = "testUser";

		private Mock<ITaskRepository> taskRepository = null;
		private Mock<ITagRepository> tagRepository = null;
		private Mock<ControllerContext> controllerContext = null;
		private Mock<IPrincipal> principal = null;

		private AjaxlessAPIController controller;

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
		public void InboxRemove()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();

			controller = new AjaxlessAPIController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.InboxRemove(1) as RedirectResult;

			// Assert
			Assert.AreEqual("/t/inbox", result.Url);
		}

		[TestMethod]
		public void Complete()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();

			controller = new AjaxlessAPIController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Complete(1, "/t") as RedirectResult;

			// Assert
			Assert.AreEqual("/t", result.Url);
			Assert.IsNotNull(tasks[0].CompletedAt);
		}

		[TestMethod]
		public void Uncomplete()
		{
			// Arrange
			tasks[0].CompletedAt = DateTime.Now;

			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();
			//tagRepository
			//	.Setup(t => t.GetMany(It.IsAny<Expression<Func<Tag, bool>>>()))
			//	.Returns(tags);

			controller = new AjaxlessAPIController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Uncomplete(1, "/t") as RedirectResult;

			// Assert
			Assert.AreEqual("/t", result.Url);
			Assert.IsNull(tasks[0].CompletedAt);
		}

		[TestMethod]
		public void AttachTag()
		{
			// Arrange
			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[1]);

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[2]);

			controller = new AjaxlessAPIController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.AttachTag(1, 4, "/t") as RedirectResult;

			// Assert
			Assert.AreEqual("/t", result.Url);
			Assert.AreEqual(true, tasks[1].Tags.Contains(tags[2]));
		}

		[TestMethod]
		public void DetachTag()
		{
			// Arrange
			tasks[0].PriorityTag = tags[0];

			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.GetWithTags(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();
			tagRepository
				.Setup(t => t.Get(It.IsAny<Expression<Func<Tag, bool>>>()))
				.Returns(tags[0]);

			controller = new AjaxlessAPIController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.DetachTag(1, 1, "/t") as RedirectResult;

			// Assert
			Assert.AreEqual("/t", result.Url);
			Assert.AreEqual(false, tasks[0].Tags.Contains(tags[0]));
		}

		[TestMethod]
		public void TogglePin()
		{
			// Arrange
			bool oldValue = tasks[0].isPinned;

			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();

			controller = new AjaxlessAPIController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.TogglePin(tasks[0].Id, "/t") as RedirectResult;
			bool newValue = tasks[0].isPinned;

			// Assert
			Assert.AreEqual("/t", result.Url);
			Assert.AreNotEqual(oldValue, newValue);
		}

		[TestMethod]
		public void ToggleProject()
		{
			// Arrange
			bool oldValue = tasks[0].isProject;

			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();

			controller = new AjaxlessAPIController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.ToggleProject(tasks[0].Id, "/t") as RedirectResult;
			bool newValue = tasks[0].isProject;

			// Assert
			Assert.AreEqual("/t", result.Url);
			Assert.AreNotEqual(oldValue, newValue);
		}

		[TestMethod]
		public void ToggleGoal()
		{
			// Arrange
			bool oldValue = tasks[0].isGoal;

			taskRepository = new Mock<ITaskRepository>();
			taskRepository
				.Setup(s => s.Get(It.IsAny<Expression<Func<Task, bool>>>()))
				.Returns(tasks[0]);

			tagRepository = new Mock<ITagRepository>();

			controller = new AjaxlessAPIController(taskRepository.Object, tagRepository.Object);
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.ToggleGoal(tasks[0].Id, "/t") as RedirectResult;
			bool newValue = tasks[0].isGoal;

			// Assert
			Assert.AreEqual("/t", result.Url);
			Assert.AreNotEqual(oldValue, newValue);
		}
	}
}