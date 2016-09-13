namespace Sorelle.Data
{
	using Configuration;
	using Model;
	using System.Data.Entity;

	public class SorelleEntities : DbContext
	{
		public SorelleEntities() : base("name=SorelleEntities")
		{
		}

		public DbSet<Task> Tasks { get; set; }
		public DbSet<Tag> Tags { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			Configuration.LazyLoadingEnabled = false;

			modelBuilder.Configurations.Add(new TaskConfiguration(modelBuilder));
			modelBuilder.Configurations.Add(new TagConfiguration(modelBuilder));
		}
	}
}