using Sorelle.Model;

namespace Sorelle.Web.Models
{
	/// <summary>
	/// Adds a boolean to Tag which reflects a selected status.
	/// </summary>
	public class TagSelect : Tag
	{
		public bool Selected { get; set; }

		public TagSelect(Tag tag, bool selected)
		{
			Id = tag.Id;
			UserId = tag.UserId;
			Name = tag.Name;
			Type = tag.Type;
			Color = tag.Color;
			Tasks = tag.Tasks;
			Selected = selected;
		}
	}
}