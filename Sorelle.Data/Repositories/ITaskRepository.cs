using Sorelle.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sorelle.Data.Repositories
{
	public interface ITaskRepository : IRepository<Task>
	{
		/// <summary>
		/// Return a Task including all associated Tasks (parent, children) and Tags.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>Sorelle.Model.Task</returns>
		Task GetDetailed(Expression<Func<Task, bool>> where);

		/// <summary>
		/// Return the next Inbox task for a given user.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>IEnumerable<Sorelle.Model.Task></returns>
		Task GetNextInboxTask(string userid);

		/// <summary>
		/// Retreive IEnumerable<Task> including Priority and Status tags.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>IEnumerable<Sorelle.Model.Task></returns>
		IEnumerable<Task> GetManyWithSpecialTags(Expression<Func<Task, bool>> where);

		/// <summary>
		/// Retreive IEnumerable<Task> including all attached Tags.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>IEnumerable<Sorelle.Model.Task></returns>
		IEnumerable<Task> GetManyWithTags(Expression<Func<Task, bool>> where);

		/// <summary>
		/// Retreive a single Task including all children tasks.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>Sorelle.Model.Task</returns>
		Task GetWithChildTasks(Expression<Func<Task, bool>> where);

		/// <summary>
		/// Retreive a single Task including all attached Tags.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>Sorelle.Model.Task</returns>
		Task GetWithTags(Expression<Func<Task, bool>> where);
	}
}