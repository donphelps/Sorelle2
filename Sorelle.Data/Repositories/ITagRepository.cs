using Sorelle.Model;
using System;
using System.Linq.Expressions;

namespace Sorelle.Data.Repositories
{
	public interface ITagRepository : IRepository<Tag>
	{
		/// <summary>
		/// Retreive a single Tag including all associated Tasks.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>Sorelle.Model.Tag</returns>
		Tag GetWithTasks(Expression<Func<Tag, bool>> where);
	}
}