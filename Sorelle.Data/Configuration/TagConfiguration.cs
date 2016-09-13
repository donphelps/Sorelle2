using Sorelle.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Sorelle.Data.Configuration
{
	internal class TagConfiguration : EntityTypeConfiguration<Tag>
	{
		public TagConfiguration(DbModelBuilder modelBuilder)
		{
			Property(t => t.Name).IsRequired();
			Property(t => t.UserId).IsRequired();
			Property(t => t.Type).IsRequired();
			Property(t => t.Color).IsRequired();

			HasMany(t => t.Tasks)
				.WithMany(t => t.Tags)
				.Map(ts =>
				{
					ts.MapLeftKey("TagRefId");
					ts.MapRightKey("TaskRefId");
					ts.ToTable("TaskTag");
				});
		}
	}
}