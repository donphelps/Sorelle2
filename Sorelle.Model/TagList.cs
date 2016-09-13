using System.Collections.Generic;

namespace Sorelle.Model
{
	/// <summary>
	/// TagList contains 5 separate tag lists, one for each Tag type.
	/// </summary>
	public class TagList
	{
		public List<Tag> Priority = new List<Tag>();
		public List<Tag> Status = new List<Tag>();
		public List<Tag> Planning = new List<Tag>();
		public List<Tag> Constraint = new List<Tag>();
		public List<Tag> Other = new List<Tag>();
	}
}