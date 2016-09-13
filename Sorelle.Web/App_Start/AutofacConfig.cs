using Autofac;
using Autofac.Integration.Mvc;
using Sorelle.Data.Infrastructure;
using Sorelle.Data.Repositories;
using Sorelle.Services;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Sorelle.Web.App_Start
{
	public class Autofac
	{
		public static void Config()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(Assembly.GetExecutingAssembly());

			builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerRequest();
			builder.RegisterAssemblyTypes(typeof(TaskRepository).Assembly)
					.Where(t => t.Name.EndsWith("Repository"))
					.AsImplementedInterfaces().InstancePerRequest();
			builder.RegisterAssemblyTypes(typeof(TaskService).Assembly)
					.Where(t => t.Name.EndsWith("Service"))
					.AsImplementedInterfaces().InstancePerRequest();

			builder.RegisterFilterProvider();
			IContainer container = builder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}