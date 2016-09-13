using Sorelle.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Sorelle.Data.Repositories
{
	public abstract class RepositoryBase<T> where T : class
	{
		private SorelleEntities dataContext;
		protected readonly IDbSet<T> dbset;

		protected RepositoryBase(IDatabaseFactory databaseFactory)
		{
			DatabaseFactory = databaseFactory;
			dbset = DataContext.Set<T>();
		}

		protected IDatabaseFactory DatabaseFactory
		{
			get;
			private set;
		}

		protected SorelleEntities DataContext
		{
			get { return dataContext ?? (dataContext = DatabaseFactory.Get()); }
		}

		public virtual void Add(T entity)
		{
			dbset.Add(entity);
		}

		public virtual void Update(T entity)
		{
			dbset.Attach(entity);
			dataContext.Entry(entity).State = EntityState.Modified;
		}

		public virtual void Delete(T entity)
		{
			dbset.Remove(entity);
		}

		public virtual void Delete(Expression<Func<T, bool>> where)
		{
			IEnumerable<T> objects = dbset.Where<T>(where).AsEnumerable();
			foreach (T obj in objects)
				dbset.Remove(obj);
		}

		public virtual T GetById(long id)
		{
			return dbset.Find(id);
		}

		public virtual T GetById(string id)
		{
			return dbset.Find(id);
		}

		public virtual IEnumerable<T> GetAll()
		{
			return dbset.ToList();
		}

		public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
		{
			return dbset.Where(where).ToList();
		}

		public T Get(Expression<Func<T, bool>> where)
		{
			return dbset.Where(where).FirstOrDefault<T>();
		}

		public void Commit()
		{
			dataContext.SaveChanges();
		}
	}
}