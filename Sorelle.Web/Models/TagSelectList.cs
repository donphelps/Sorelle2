using Sorelle.Model;
using System.Collections.Generic;

namespace Sorelle.Web.Models
{
	/// <summary>
	/// Tag list containing TagSelects separated by Tag type.
	/// </summary>
	public class TagSelectList
	{
		public List<TagSelect> Priority;
		public List<TagSelect> Status;
		public List<TagSelect> Planning;
		public List<TagSelect> Constraint;
		public List<TagSelect> Other;

		public TagSelectList(Task task, TagList tagList)
		{
			Priority = new List<TagSelect>();
			Status = new List<TagSelect>();
			Planning = new List<TagSelect>();
			Constraint = new List<TagSelect>();
			Other = new List<TagSelect>();

			foreach (Tag tag in tagList.Priority)
			{
				Priority.Add(new TagSelect(tag, (task.PriorityTag?.Id == tag.Id)));
			}

			foreach (Tag tag in tagList.Status)
			{
				Status.Add(new TagSelect(tag, (task.StatusTag?.Id == tag.Id)));
			}

			foreach (Tag tag in tagList.Planning)
			{
				Planning.Add(new TagSelect(tag, (task.Tags.Contains(tag))));
			}

			foreach (Tag tag in tagList.Constraint)
			{
				Constraint.Add(new TagSelect(tag, (task.Tags.Contains(tag))));
			}

			foreach (Tag tag in tagList.Other)
			{
				Other.Add(new TagSelect(tag, (task.Tags.Contains(tag))));
			}
		}
	}
}