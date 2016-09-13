using Sorelle.Model.Enums;
using System.Collections.Generic;

namespace Sorelle.Model
{
	public class Tag
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		public string Name { get; set; }

		public TagType Type { get; set; }

		public TagColor Color { get; set; }

		public virtual ICollection<Task> Tasks { get; set; }

		public Tag()
		{
		}

		public Tag(string name, string userid, TagType type, TagColor color)
		{
			Name = name;
			UserId = userid;
			Type = type;
			Color = color;
		}
	}
}