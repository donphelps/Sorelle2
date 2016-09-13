using Sorelle.Data.Repositories;
using Sorelle.Model;
using Sorelle.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Sorelle.Services
{
	public class TagService
	{
		private ITagRepository tagRepository;
		private ITaskRepository taskRepository;

		public TagService(ITagRepository tagRepository, ITaskRepository taskRepository)
		{
			this.tagRepository = tagRepository;
			this.taskRepository = taskRepository;
		}

		public void Create(Tag tag, string userid)
		{
			tag.UserId = userid;
			tagRepository.Add(tag);
			commit();
		}

		public void CreateNewUserTagSet(string userid)
		{
			tagRepository.Add(new Tag("Immediate", userid, TagType.Priority, TagColor.Red));
			tagRepository.Add(new Tag("Next", userid, TagType.Priority, TagColor.Orange));
			tagRepository.Add(new Tag("Queue", userid, TagType.Priority, TagColor.Yellow));
			tagRepository.Add(new Tag("Soon", userid, TagType.Priority, TagColor.GreenLt));

			tagRepository.Add(new Tag("Backlog", userid, TagType.Status, TagColor.Default));
			tagRepository.Add(new Tag("Research", userid, TagType.Status, TagColor.Blue));
			tagRepository.Add(new Tag("Planning", userid, TagType.Status, TagColor.BlueLt));
			tagRepository.Add(new Tag("Testing", userid, TagType.Status, TagColor.Cyan));

			tagRepository.Add(new Tag("Daily List", userid, TagType.Planning, TagColor.Orange));
			tagRepository.Add(new Tag("Weekly List", userid, TagType.Planning, TagColor.Yellow));
			tagRepository.Add(new Tag("This Season", userid, TagType.Planning, TagColor.Green));
			tagRepository.Add(new Tag("2016", userid, TagType.Planning, TagColor.Cyan));

			tagRepository.Add(new Tag("Work", userid, TagType.Constraint, TagColor.Blue));
			tagRepository.Add(new Tag("Home", userid, TagType.Constraint, TagColor.BlueLt));
			tagRepository.Add(new Tag("Internet", userid, TagType.Constraint, TagColor.Cyan));

			commit();
		}

		public void Delete(int id, string userid)
		{
			Tag tag = tagRepository.Get(t => t.Id == id && t.UserId == userid);

			// Disassociate all tasks from the tag we're trying to delete.
			var tasksWithTag = taskRepository.GetManyWithTags(t => t.Id == id && t.UserId == userid);
			foreach (var task in tasksWithTag)
			{
				if (task.PriorityTag?.Id == id)
					task.PriorityTag = null;
				if (task.StatusTag?.Id == id)
					task.StatusTag = null;
				if (task.Tags.Contains(tag))
					task.Tags.Remove(tag);
			}

			tagRepository.Delete(tag);
			commit();
		}

		public bool Edit(Tag tag, string userid)
		{
			Tag original = tagRepository.Get(t => t.Id == tag.Id && t.UserId == userid);

			if (original == null)
				return false;

			original.Color = tag.Color;
			original.Name = tag.Name;
			original.Type = tag.Type;

			commit();
			return true;
		}

		public Tag Get(int id, string userid)
		{
			return tagRepository.Get(t => t.Id == id && t.UserId == userid);
		}

		public TagList GetAll(string userid)
		{
			TagList result = new TagList();
			var tags = tagRepository.GetMany(t => t.UserId == userid);

			result.Priority = tags.Where(t => t.Type == TagType.Priority).OrderBy(t => t.Color).ToList();
			result.Status = tags.Where(t => t.Type == TagType.Status).OrderBy(t => t.Color).ToList();
			result.Planning = tags.Where(t => t.Type == TagType.Planning).OrderBy(t => t.Color).ToList();
			result.Constraint = tags.Where(t => t.Type == TagType.Constraint).OrderBy(t => t.Color).ToList();
			result.Other = tags.Where(t => t.Type == TagType.Other).OrderBy(t => t.Color).ToList();

			return result;
		}

		public Tag GetWithTasks(int id, string userid)
		{
			var tag = tagRepository.GetWithTasks(t => t.Id == id && t.UserId == userid);

			if (tag.Type == TagType.Priority)
				tag.Tasks = taskRepository
					.GetManyWithSpecialTags(t => t.PriorityTag.Id == tag.Id && t.CompletedAt == null && t.DeletedAt == null)
					.OrderByDescending(o => o.isPinned)
					.ThenByDescending(o => o.DueDate)
					.ThenBy(o => o.PriorityTag == null)
					.ThenBy(o => o.PriorityTag?.Color)
					.ToList();
			else if (tag.Type == TagType.Status)
				tag.Tasks = taskRepository.
					GetManyWithSpecialTags(t => t.StatusTag.Id == tag.Id && t.CompletedAt == null && t.DeletedAt == null)
					.OrderByDescending(o => o.isPinned)
					.ThenByDescending(o => o.DueDate)
					.ThenBy(o => o.PriorityTag == null)
					.ThenBy(o => o.PriorityTag?.Color)
					.ToList();
			else
				tag.Tasks = tag.Tasks
					.Where(t => t.CompletedAt != null && t.DeletedAt != null)
					.OrderByDescending(o => o.isPinned)
					.ThenByDescending(o => o.DueDate)
					.ThenBy(o => o.PriorityTag == null)
					.ThenBy(o => o.PriorityTag?.Color)
					.ToList();

			return tag;
		}

		/// <summary>
		/// Selects the list priority tags.
		/// </summary>
		/// <param name="userid">The userid.</param>
		/// <returns></returns>
		public List<Tag> SelectListPriorityTags(string userid)
		{
			return tagRepository
				.GetMany(t => t.UserId == userid && t.Type == TagType.Priority)
				.OrderBy(t => t.Color)
				.ToList();
		}

		/// <summary>
		/// Selects the list status tags.
		/// </summary>
		/// <param name="userid">The userid.</param>
		/// <returns></returns>
		public List<Tag> SelectListStatusTags(string userid)
		{
			return tagRepository
				.GetMany(t => t.UserId == userid && t.Type == TagType.Status)
				.OrderBy(t => t.Color)
				.ToList();
		}

		private void commit()
		{
			tagRepository.Commit();
		}
	}
}