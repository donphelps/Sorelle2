using System;

namespace Sorelle.Data.Infrastructure
{
	public interface IDatabaseFactory : IDisposable
	{
		SorelleEntities Get();
	}
}