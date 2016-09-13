using Sorelle.Data.Infrastructure;
using Sorelle.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Sorelle.Data.Repositories
{
	public class TaskRepository : RepositoryBase<Task>, ITaskRepository
	{
		public TaskRepository(IDatabaseFactory databaseFactory)
				: base(databaseFactory)
		{
		}

		/// <summary>
		/// Return a Task including all associated Tasks (parent, children) and Tags.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>
		/// Sorelle.Model.Task
		/// </returns>
		public Task GetDetailed(Expression<Func<Task, bool>> where)
		{
			Task task = dbset.Where(where)
				.Include(t => t.ParentTask)
				.Include(t => t.Tasks)
				.Include(t => t.PriorityTag)
				.Include(t => t.StatusTag)
				.Include(t => t.Tags)
				.FirstOrDefault();

			task.Tasks = task.Tasks
				.Where(t => t.CompletedAt == null && t.DeletedAt == null)
				.OrderByDescending(o => o.isPinned)
				.ThenByDescending(o => o.DueDate)
				.ThenBy(o => o.PriorityTag == null)
				.ThenByDescending(o => o.PriorityTag?.Color)
				.ToList();

			return task;
		}

		public Task GetNextInboxTask(string userid)
		{
			var task = dbset.Where(t => t.Inboxed && t.UserId == userid)
				.Include(t => t.ParentTask)
				.Include(t => t.Tasks)
				.Include(t => t.PriorityTag)
				.Include(t => t.StatusTag)
				.Include(t => t.Tags)
				.OrderBy(t => t.Id)
				.FirstOrDefault();

			task.Tasks = task.Tasks
				.Where(t => t.CompletedAt == null && t.DeletedAt == null)
				.OrderByDescending(o => o.isPinned)
				.ThenByDescending(o => o.DueDate)
				.ThenBy(o => o.PriorityTag == null)
				.ThenByDescending(o => o.PriorityTag?.Color)
				.ToList();

			return task;
		}

		/// <summary>
		/// Retreive a single Task including all children tasks.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>
		/// Sorelle.Model.Task
		/// </returns>
		public Task GetWithChildTasks(Expression<Func<Task, bool>> where)
		{
			return dbset.Where(where)
				.Include(t => t.Tasks)
				.FirstOrDefault();
		}

		/// <summary>
		/// Retreive a single Task including all attached Tags.
		/// </summary>
		/// <param name="where">Where clause lambda.</param>
		/// <returns>
		/// Sorelle.Model.Task
		/// </returns>
		public Task GetWithTags(Expression<Func<Task, bool>> where)
		{
			return dbset.Where(where)
				.Include(t => t.PriorityTag)
				.Include(t => t.StatusTag)
				.Include(t => t.Tags)
				.FirstOrDefault();
		}

		/// <summary>
		/// Gets the many with special tags.
		/// </summary>
		/// <param name="where">The where.</param>
		/// <returns></returns>
		public IEnumerable<Task> GetManyWithSpecialTags(Expression<Func<Task, bool>> where)
		{
			return dbset.Where(where)
				.Include(t => t.PriorityTag)
				.Include(t => t.StatusTag);
		}

		/// <summary>
		/// Gets the many with tags.
		/// </summary>
		/// <param name="where">The where.</param>
		/// <returns></returns>
		public IEnumerable<Task> GetManyWithTags(Expression<Func<Task, bool>> where)
		{
			return dbset.Where(where)
				.Include(t => t.PriorityTag)
				.Include(t => t.StatusTag)
				.Include(t => t.Tags);
		}
	}
}