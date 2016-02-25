using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;

namespace Repository
{
    public class EFRepository<T> : IRepository<T> 
        where T : class
         
    {
        private TraktatEntities _ctx;

        public EFRepository(TraktatEntities ctx)
        {
            _ctx = ctx;
        }

        public T Find(int id)
        {
            return _ctx.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _ctx.Set<T>().AsEnumerable<T>();
        }

        public IQueryable<T> Query()
        {
            return _ctx.Set<T>().AsQueryable<T>();
        }

        public int AddOrUpdate(T entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(T entity)
        {
            throw new NotImplementedException();
        }


        public IQueryable<T1> Query<T1>()
            where T1: class
        {
            return _ctx.Set<T1>();
        }
    }
}
