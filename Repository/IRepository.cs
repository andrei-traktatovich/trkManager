using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;

namespace Repository
{
    public interface IRepository<T> where T : class 
    {
        T Find(int id);
        IEnumerable<T> GetAll();
        IQueryable<T> Query();
        int AddOrUpdate(T entity);
        bool Delete(T entity);

        IQueryable<T1> Query<T1>() 
            where T1 : class;
    }
}
