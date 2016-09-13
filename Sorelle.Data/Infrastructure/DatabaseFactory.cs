namespace Sorelle.Data.Infrastructure
{
	public class DatabaseFactory : Disposable, IDatabaseFactory
	{
		private SorelleEntities dataContext;

		public SorelleEntities Get()
		{
			return dataContext ?? (dataContext = new SorelleEntities());
		}

		protected override void DisposeCore()
		{
			if (dataContext != null)
				dataContext.Dispose();
		}
	}
}