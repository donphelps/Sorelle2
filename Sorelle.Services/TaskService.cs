using Sorelle.Data.Repositories;
using Sorelle.Model;
using Sorelle.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sorelle.Services
{
	public class TaskService
	{
		private ITaskRepository taskRepository;
		private ITagRepository tagRepository;

		public TaskService(ITaskRepository taskRepository, ITagRepository tagRepository)
		{
			this.taskRepository = taskRepository;
			this.tagRepository = tagRepository;
		}

		public bool AttachTag(int taskid, int tagid, string userid)
		{
			Task task = taskRepository.Get(t => t.Id == taskid && t.UserId == userid);
			Tag tag = tagRepository.Get(t => t.Id == tagid && t.UserId == userid);

			if (task == null || tag == null)
				return false;

			switch (tag.Type)
			{
				case TagType.Priority:
					task.PriorityTag = tag;
					break;

				case TagType.Status:
					task.StatusTag = tag;
					break;

				default:
					task.Tags.Add(tag);
					break;
			}

			commit();
			return true;
		}

		/// <summary>
		/// Mark the Task and all child Tasks as complete.
		/// </summary>
		/// <param name="id">Task Id</param>
		/// <param name="userid">User Id</param>
		/// <returns>
		/// Success
		/// </returns>
		public bool Complete(int id, string userid)
		{
			Task task = taskRepository.Get(t => t.Id == id && t.UserId == userid);

			if (task == null)
				return false;

			complete(task);

			commit();
			return true;
		}

		public void Create(Task task, string userid)
		{
			task.UserId = userid;
			task.Inboxed = true;
			taskRepository.Add(task);
			commit();
		}

		public void CreateNewUserTaskSet(string userid)
		{
			taskRepository.Add(new Task("Get to know the UI", userid));
			taskRepository.Add(new Task("Process an item in your Inbox", userid));
			taskRepository.Add(new Task("Mark a task as a project", userid));
			taskRepository.Add(new Task("Pin a Task", userid));
			commit();
		}

		public bool DetachTag(int taskid, int tagid, string userid)
		{
			Task task = taskRepository.GetWithTags(t => t.Id == taskid && t.UserId == userid);
			Tag tag = tagRepository.Get(t => t.Id == tagid && t.UserId == userid);

			if (task == null || tag == null)
				return false;

			switch (tag.Type)
			{
				case TagType.Priority:
					task.PriorityTag = null;
					break;

				case TagType.Status:
					task.StatusTag = null;
					break;

				default:
					task.Tags.Remove(tag);
					break;
			}

			commit();
			return true;
		}

		public bool Edit(Task task, string userid)
		{
			Task original = taskRepository.Get(t => t.Id == task.Id && t.UserId == userid);

			if (original != null)
			{
				original.Name = task.Name;
				original.Notes = task.Notes;
				original.Link = task.Link;
				original.isGoal = task.isGoal;
				original.isPinned = task.isPinned;
				original.isProject = task.isProject;
				original.Inboxed = task.Inboxed;
				original.DueDate = task.DueDate;
				original.CompletedAt = task.CompletedAt;
				original.DeletedAt = task.DeletedAt;

				commit();
				return true;
			}
			else
				return false;
		}

		public Task Get(int id, string userid)
		{
			return taskRepository.Get(t => t.Id == id && t.UserId == userid);
		}

		public IEnumerable<Task> GetAllByDueDate(string userid)
		{
			return taskRepository.GetManyWithSpecialTags(t => t.UserId == userid
					&& t.CompletedAt == null
					&& t.DeletedAt == null
					&& t.DueDate != null)
				.OrderByDescending(t => t.DueDate)
				.ThenBy(t => t.PriorityTag?.Color);
		}

		public IEnumerable<Task> GetAllGoals(string userid)
		{
			return taskRepository
				.GetManyWithSpecialTags(
					t => t.UserId == userid
					&& t.CompletedAt == null
					&& t.DeletedAt == null
					&& t.isGoal)
				.OrderByDescending(o => o.isPinned)
				.ThenByDescending(o => o.DueDate)
				.ThenBy(o => o.PriorityTag == null)
				.ThenBy(o => o.PriorityTag?.Color);
		}

		public IEnumerable<Task> GetAllIncomplete(string userid)
		{
			return taskRepository.GetManyWithSpecialTags(t => t.UserId == userid && t.CompletedAt == null && t.DeletedAt == null);
		}

		public IEnumerable<Task> GetAllProjects(string userid)
		{
			return taskRepository
				.GetManyWithSpecialTags(
					t => t.UserId == userid
					&& t.CompletedAt == null
					&& t.DeletedAt == null
					&& t.isProject)
				.OrderByDescending(o => o.isPinned)
				.ThenByDescending(o => o.DueDate)
				.ThenBy(o => o.PriorityTag == null)
				.ThenBy(o => o.PriorityTag?.Color);
		}

		public IEnumerable<Task> GetAllRecentlyCompleted(string userid)
		{
			return taskRepository
				.GetManyWithSpecialTags(
					t => t.UserId == userid
					&& t.CompletedAt != null
					&& t.DeletedAt == null)
				.OrderByDescending(o => o.isPinned)
				.ThenByDescending(o => o.DueDate)
				.ThenBy(o => o.PriorityTag == null)
				.ThenBy(o => o.PriorityTag?.Color);
		}

		public IEnumerable<Task> GetAllRoot(string userid)
		{
			return taskRepository
				.GetManyWithSpecialTags(
					t => t.UserId == userid
					&& t.CompletedAt == null
					&& t.DeletedAt == null
					&& t.ParentTask == null
					&& !t.isGoal)
				.OrderByDescending(o => o.isPinned)
				.ThenByDescending(o => o.DueDate)
				.ThenBy(o => o.PriorityTag == null)
				.ThenBy(o => o.PriorityTag?.Color);
		}

		/// <summary>
		/// Return a Task including all associated Tasks (parent, children) and Tags.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userid"></param>
		/// <returns>
		/// Sorelle.Model.Task
		/// </returns>
		public Task GetDetailed(int id, string userid)
		{
			Task task = taskRepository
				.GetDetailed(t => t.Id == id && t.UserId == userid);

			return task;
		}

		/// <summary>
		/// Retreives the next inboxed Task Id.
		/// </summary>
		/// <param name="userid">User Id</param>
		/// <returns>
		/// Id of the next Inbox Task as int
		/// </returns>
		public Task GetNextInboxTask(string userid)
		{
			return taskRepository.GetNextInboxTask(userid);
		}

		/// <summary>
		/// Removes the Inbox flag from a Task.
		/// </summary>
		/// <param name="id">Task Id</param>
		/// <param name="userid">User Id</param>
		/// <returns>
		/// Success
		/// </returns>
		public bool InboxRemove(int id, string userid)
		{
			var task = taskRepository.Get(t => t.Id == id && t.UserId == userid);

			if (task == null)
				return false;

			task.Inboxed = false;

			commit();
			return true;
		}

		public void QuickAddAsGoal(Task task, string userid)
		{
			task.UserId = userid;
			task.Inboxed = true;
			task.isGoal = true;
			taskRepository.Add(task);
			commit();
		}

		public void QuickAddAsProject(Task task, string userid)
		{
			task.UserId = userid;
			task.Inboxed = true;
			task.isProject = true;
			taskRepository.Add(task);
			commit();
		}

		public bool QuickAddWithParent(Task task, int parentId, string userid)
		{
			var parent = taskRepository.Get(t => t.Id == parentId && t.UserId == userid);
			if (parent == null)
				return false;

			task.UserId = userid;
			task.Inboxed = true;
			task.ParentTask = parent;

			taskRepository.Add(task);
			commit();
			return true;
		}

		public bool QuickAddWithTag(Task task, int tagId, string userid)
		{
			var tag = tagRepository.Get(t => t.Id == tagId && t.UserId == userid);

			if (tag == null)
				return false;

			switch (tag.Type)
			{
				case TagType.Priority:
					task.PriorityTag = tag;
					break;

				case TagType.Status:
					task.StatusTag = tag;
					break;

				default:
					task.Tags.Add(tag);
					break;
			}

			task.UserId = userid;
			task.Inboxed = true;

			taskRepository.Add(task);
			commit();
			return true;
		}

		/// <summary>
		/// Flag all completed Tasks as deleted.
		/// </summary>
		/// <param name="userid">User Id</param>
		public void RemoveAllComplete(string userid)
		{
			var list = taskRepository.GetMany(t => t.CompletedAt != null && t.UserId == userid);

			foreach (var item in list)
			{
				item.DeletedAt = DateTime.Now;
			}

			commit();
		}

		public bool ToggleGoal(int id, string userid)
		{
			var task = taskRepository.Get(t => t.Id == id && t.UserId == userid);

			if (task == null)
				return false;

			task.isGoal = !task.isGoal;

			commit();
			return true;
		}

		public bool TogglePin(int id, string userid)
		{
			var task = taskRepository.Get(t => t.Id == id && t.UserId == userid);

			if (task == null)
				return false;

			task.isPinned = !task.isPinned;

			commit();
			return true;
		}

		public bool ToggleProject(int id, string userid)
		{
			var task = taskRepository.Get(t => t.Id == id && t.UserId == userid);

			if (task == null)
				return false;

			task.isProject = !task.isProject;

			commit();
			return true;
		}

		/// <summary>
		/// Mark the Task and all child Tasks as not complete.
		/// </summary>
		/// <param name="id">Task Id</param>
		/// <param name="userid">User Id</param>
		/// <returns>
		/// Success
		/// </returns>
		public bool Uncomplete(int id, string userid)
		{
			Task task = taskRepository.Get(t => t.Id == id && t.UserId == userid);

			if (task == null)
				return false;

			uncomplete(task);

			commit();
			return true;
		}

		private void commit()
		{
			taskRepository.Commit();
		}

		/// <summary>
		/// Recursively complete Task and all child Tasks.
		/// </summary>
		/// <param name="task">The Task.</param>
		private void complete(Task task)
		{
			task.CompletedAt = DateTime.Now;
			task.Inboxed = false;

			var children = taskRepository.GetMany(t => t.ParentTask.Id == task.Id);

			if (children != null)
			{
				foreach (Task child in children)
				{
					complete(child);
				}
			}
		}

		/// <summary>
		/// Recursively mark not complete Task and all child Tasks.
		/// </summary>
		/// <param name="task">The Task.</param>
		private void uncomplete(Task task)
		{
			task.CompletedAt = null;

			var children = taskRepository.GetMany(t => t.ParentTask.Id == task.Id);

			if (children != null)
			{
				foreach (Task child in children)
				{
					uncomplete(child);
				}
			}
		}
	}
}