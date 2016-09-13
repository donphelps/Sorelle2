using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Principal;
using System.Web.Mvc;

namespace Sorelle.Web.Test.Controllers
{
	[TestClass]
	public class HomeControllerTests
	{
		private Mock<ControllerContext> controllerContext = null;
		private Mock<IPrincipal> principal = null;

		[TestInitialize]
		public void Init()
		{
			controllerContext = new Mock<ControllerContext>();
			principal = new Mock<IPrincipal>();
			principal.SetupGet(x => x.Identity.IsAuthenticated).Returns(false);
			controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
		}

		[TestMethod]
		public void Index()
		{
			// Arrange
			var controller = new Sorelle.Web.Controllers.HomeController();
			controller.ControllerContext = controllerContext.Object;

			// Act
			var result = controller.Index() as ViewResult;

			// Assert
			Assert.AreEqual("Sorelle", result.ViewBag.Title);
			Assert.IsNotNull(result);
		}
	}
}