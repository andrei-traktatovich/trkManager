using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;
using TicketDataModel;
using System.Data.Entity;

namespace BusinessLogic.TimeSheets
{
    public static class TranslatorRepoExt
    {
        public static IQueryable<Translator> GetActiveEmployees(this IRepository<Translator> @this, 
            IEnumerable<int> officeIds, DateTime start, DateTime? end)
        {
            var endDate = end ?? start;
            
            endDate = endDate.Date;
            start = start.Date;

            var permanents = @this.GetPermanent(officeIds);

            var temporaries = @this.GetTemporary(officeIds, start, endDate);

            var result = permanents.Union(temporaries).Distinct();

            return result;
        }

        public static IQueryable<Translator> StaffEmployees(this IQueryable<Translator> @this)
        {
            return @this
                .Individuals()
                .Where(x => x.TitleStatusID == 1);
        }

        public static IQueryable<Translator> GetTemporary(this IRepository<Translator> @this, IEnumerable<int> officeIds, DateTime startDate, DateTime endDate)
        {
            return @this
                .Query()
                .Individuals()
                .Where(x => x.CalendarPeriods.Any(y => y.StartDate.Value <= endDate && y.EndDate.Value >= startDate && officeIds.Contains(y.OfficeID)));
        }
        public static IQueryable<Translator> Individuals(this IQueryable<Translator> @this)
        {
            return @this.Where(x => x.ReferenceOffice == null && x.TitleID != 4);
        }

        public static IQueryable<Translator> Existing(this IQueryable<Translator> @this)
        {
            return @this.Where(x => x.s_del == 0);
        }

        public static IQueryable<Translator> Active(this IQueryable<Translator> @this)
        {
            return @this.Where(x => x.active.ToLower() != "резерв"
                        && x.active.ToLower() != "черный список");
        }

        public static IQueryable<Translator> WithPermanentOffice(this IQueryable<Translator> @this, IEnumerable<int> officeIds)
        {
            return @this.Where(x => officeIds.Contains(x.OfficeID.Value));
        }
        public static IQueryable<Translator> GetPermanent(this IRepository<Translator> @this, IEnumerable<int> officeIds)
        {
            return @this.Query()
                      .StaffEmployees()
                      .Existing()
                      .WithPermanentOffice(officeIds)
                      .Active();
        }
    }

    public static class OfficeRepoExt
    {
        public static IQueryable<CalendarPeriod> GetCalendarPeriods(this IRepository<Office> @this, IEnumerable<int> officeIds, DateTime startDate, DateTime endDate)
        {
            return @this.Query<CalendarPeriod>()
                .Include(x => x.Translator.Title)
                .Include(x => x.Office)
                .Include(x => x.StaffStatusEntity)
                .InOffice(officeIds)
                .Between(startDate, endDate)
                .Confirmed();
        }

        public static IQueryable<IGrouping<Translator, CalendarPeriod>> ByEmployee(this IQueryable<CalendarPeriod> @this)
        {
            return @this.GroupBy(x => x.Translator);
        }

        public static IQueryable<CalendarPeriod> InOffice(this IQueryable<CalendarPeriod> @this, IEnumerable<int> officeIds)
        {
            return @this.Where(x => officeIds.Contains(x.OfficeID));
        }

        public static IQueryable<CalendarPeriod> Between(this IQueryable<CalendarPeriod> @this, DateTime start, DateTime end)
        {
            return @this.Where(x => x.EndDate.HasValue && x.EndDate.Value >= start && x.StartDate.HasValue && x.StartDate.Value <= end);
        }

        public static IQueryable<CalendarPeriod> Confirmed(this IQueryable<CalendarPeriod> @this)
        {
            return @this.Where(x => x.Confirmed);
        }
        
    }
    public class WorkDaySheetBuilder
    {
        IRepository<Office> _officeRepo;
        IRepository<Translator> _translatorRepo;

        public WorkDaySheetBuilder(IRepository<Office> officeRepo, IRepository<Translator> translatorRepo)
        {
            if (officeRepo == null)
                throw new ArgumentNullException("officeRepo");
            _officeRepo = officeRepo;

            if (translatorRepo == null)
                throw new ArgumentNullException("translatorRepo");
            _translatorRepo = translatorRepo;
        }

        public WorkDaySheet BuildAll(DateTime startDate, DateTime endDate)
        {
            return GetAll(startDate, endDate);
        }

        public WorkDaySheet BuildSheet(int[] ids, DateTime startDate, DateTime endDate, string name = "")
        {
            return GetSheet(ids, startDate, endDate, name);
        }

        public List<WorkDaySheet> BuildSheets(int[] ids, DateTime startDate, DateTime endDate)
        {
            return this.GetSheets(ids, startDate, endDate);
        }

        private WorkDaySheet GetAll(DateTime startDate, DateTime endDate)
        {
            var ids = _officeRepo.GetAll().GetActiveOfficeList().Select(x => x.ID).ToArray();
            var sheet = GetConsolidatedSheet(ids, startDate, endDate, "Сводная");
            return sheet;
        }

        private WorkDaySheet GetSheet(int[] ids, DateTime startDate, DateTime endDate, string name = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                var namesList = _officeRepo
                    .Query()
                    .Where(x => ids.Contains(x.ID))
                    .Select(x => x.Name)
                    .OrderBy(x => x);

                name = string.Join(", ", namesList);
            }

            var sheet = GetConsolidatedSheet(ids, startDate, endDate, name);

            return sheet;
        }

        private WorkDaySheet GetConsolidatedSheet(int[] officeIds, DateTime startDate, DateTime endDate, string name)
        {
             
            
            var calendarPeriods = _officeRepo
                .GetCalendarPeriods(officeIds, startDate, endDate);

            var sheet = WorkDaySheet.Build(calendarPeriods, startDate, endDate, name);
            return sheet;
        }

        private List<WorkDaySheet> GetSheets(int[] ids, DateTime startDate, DateTime endDate)
        {
            
            var result = new List<WorkDaySheet>();
            foreach (var id in ids)
            {
                var office = _officeRepo.Find(id);
                if (office == null)
                    continue; // fail silently!!!

                var calendarPeriods = _officeRepo
                .GetCalendarPeriods(new int[] { id }, startDate, endDate);

                var sheet = WorkDaySheet.Build(calendarPeriods, startDate, endDate, office.Name);
                
                result.Add(sheet);
            }
            return result;
        }
    }
}
