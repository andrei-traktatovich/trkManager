using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TicketDataModel
{
    public interface IIdName<T>
    {
        T ID { get; set; }
        string Name { get; set; }
    }

    public interface IIdName : IIdName<int> { }

    public partial class Office : IIdName { }
    
    
    public partial class StaffStatus : IIdName<StaffStatuses> { }

    public class IdNamePair
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static Expression<Func<IIdName<T>, IdNamePair>> ToIdNamePair<T>()
        {
            return x => new IdNamePair { Id = x.ID as int? ?? 0, Name = x.Name };
        }
        public static Expression<Func<IIdName, IdNamePair>> ToIdNamePair()
        {
            return x => new IdNamePair { Id = x.ID, Name = x.Name };
        }
    }

    public static class IdNameExtensions
    {
        public static IQueryable<IdNamePair> ToIdNamePairs(this IQueryable<IIdName> @this)
        {
            return @this.Select(IdNamePair.From<IIdName);
        }
    }
}
