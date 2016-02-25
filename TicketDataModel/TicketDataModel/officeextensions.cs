using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace TicketDataModel
{
    

    public static class TranslatorExtensionMethods
    {
        public static IQueryable<Translator> Permanent(this IQueryable<Translator> translators)
        {
            return translators.PermanentStaffEmployees();
        }

        public static IQueryable<Translator> PermanentStaffEmployees(this IQueryable<Translator> translators)
        {
            return translators
                .Where(x => x.TitleStatusID == 1 && x.s_del == 0
                        && x.ReferenceOffice == null && x.TitleID != 4
                        && x.active.ToLower() != "резерв"
                        && x.active.ToLower() != "черный список");
        }

        public static IQueryable<Translator> GetActiveEmployees(this IQueryable<Translator> employees, IEnumerable<int> officeIDs = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            startDate = startDate ?? DateTime.Now;

            endDate = endDate ?? startDate.Value;

            var permanent = employees.Permanent().Where(x => officeIDs.Contains(x.OfficeID.Value));

            var temporary = employees.TemporaryEmployees(startDate.Value, endDate.Value);
            
            if (employees != null)
                temporary = temporary.Where(x => officeIDs.Contains(x.OfficeID.Value));


            var result = (temporary != null ? permanent.Union(temporary) : permanent)
                .Where(x => x != null);

            return result;
            
        }

        public static IQueryable<Translator> TemporaryEmployees(this IQueryable<Translator> employees, DateTime startDate, DateTime endDate)
        {
            var result = employees.Where(x => x.CalendarPeriods.Any(y => y.StartDate.Value <= endDate && y.EndDate.Value >= startDate));
            return result; 
        }
    }

    public static class OfficeExtensionMethods
    {
        public static IEnumerable<Office> GetActiveOfficeList(this IEnumerable<Office> offices)
        {
            return offices.Where(x => x.pod_rod == 1 && x.Del == 0 && x.IsInHouseOffice).OrderBy(x => x.Name);
        }
    }
}
