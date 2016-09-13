using System.ComponentModel.DataAnnotations;

namespace Sorelle.Web.Models
{
	public class QuickAddViewModel
	{
		[Required]
		public string Name { get; set; }

		public int? ParentId { get; set; }
		public int? TagId { get; set; }
		public bool Project { get; set; }
		public bool Goal { get; set; }
		public string ReturnUrl { get; set; }

		public QuickAddViewModel()
		{
			ParentId = null;
			TagId = null;
		}

		public QuickAddViewModel(string name, int? parentId, int? tagId, bool project, bool goal, string returnurl)
		{
			Name = name;
			ParentId = parentId;
			TagId = tagId;
			Project = project;
			Goal = goal;
			ReturnUrl = returnurl;
		}
	}
}