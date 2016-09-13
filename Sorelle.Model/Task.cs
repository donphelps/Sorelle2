using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sorelle.Model
{
	public class Task
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		[Display(Name = "Task")]
		public string Name { get; set; }

		public string Notes { get; set; }
		public string Link { get; set; }

		[Display(Name = "Project")]
		public bool isProject { get; set; }

		[Display(Name = "Goal")]
		public bool isGoal { get; set; }

		[Display(Name = "Pinned")]
		public bool isPinned { get; set; }

		[Display(Name = "In Inbox")]
		public bool Inboxed { get; set; }

		[Display(Name = "Due Date")]
		public DateTime? DueDate { get; set; }

		[Display(Name = "Completed On")]
		public DateTime? CompletedAt { get; set; }

		[Display(Name = "Deleted On")]
		public DateTime? DeletedAt { get; set; }

		public virtual Task ParentTask { get; set; }
		public virtual ICollection<Task> Tasks { get; set; }

		public virtual Tag PriorityTag { get; set; }
		public virtual Tag StatusTag { get; set; }
		public virtual ICollection<Tag> Tags { get; set; }

		public Task()
		{
			CompletedAt = null;
			DeletedAt = null;
			Inboxed = true;

			Tasks = new List<Task>();
			Tags = new List<Tag>();
		}

		public Task(string name, string userid)
		{
			Name = name;
			UserId = userid;

			CompletedAt = null;
			DeletedAt = null;
			Inboxed = true;

			Tasks = new List<Task>();
			Tags = new List<Tag>();
		}
	}
}