using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ticket_Tracker.DAL.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal ApplicationDbContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository()
        {
            this.context = new ApplicationDbContext();
            this.dbSet = context.Set<TEntity>();
        }

        public GenericRepository(ApplicationDbContext _context)
        {
            this.context = _context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> GetAllRecords()
        {
            return this.dbSet.ToList();
        }

        public virtual TEntity GetSingleRecord(int id)
        {
            return this.dbSet.Find(id);
        }

        public virtual TEntity LatestRecord()
        {
            return this.dbSet.ToList().Last();
        }

        public virtual void AddRecord(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        public virtual void UpdateRecord(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void DeleteRecord(TEntity entity)
        {
            this.dbSet.Remove(entity);
        }
    }
}