using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketDataModel
{
    public partial class Customer
    {
        public static Customer GetByID(int id)
        {
            using (var ctx = new TraktatEntities())
            {
                return ctx.Customers.Where(x => x.ID == id).SingleOrDefault();
            }
        }
    }
}
