using Sorelle.Data.Infrastructure;
using Sorelle.Model;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Sorelle.Data.Repositories
{
	public class TagRepository : RepositoryBase<Tag>, ITagRepository
	{
		public TagRepository(IDatabaseFactory databaseFactory)
				: base(databaseFactory)
		{
		}

		/// <summary>
		/// Retreive a single Tag including all associated Tasks.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>
		/// Sorelle.Model.Tag
		/// </returns>
		public Tag GetWithTasks(Expression<Func<Tag, bool>> where)
		{
			return dbset.Where(where).Include(t => t.Tasks).FirstOrDefault();
		}
	}
}