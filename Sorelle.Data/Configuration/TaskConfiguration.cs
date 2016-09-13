using Sorelle.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Sorelle.Data.Configuration
{
	internal class TaskConfiguration : EntityTypeConfiguration<Task>
	{
		public TaskConfiguration(DbModelBuilder modelBuilder)
		{
			Property(t => t.Name).IsRequired();
			Property(t => t.UserId).IsRequired();

			Property(t => t.CompletedAt).IsOptional();
			Property(t => t.DeletedAt).IsOptional();

			HasMany(t => t.Tags)
				.WithMany(t => t.Tasks)
				.Map(ts =>
				{
					ts.MapLeftKey("TaskRefId");
					ts.MapRightKey("TagRefId");
					ts.ToTable("TaskTag");
				});
		}
	}
}