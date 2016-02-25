using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;


namespace TicketDataModel
{
    public static class TranslatorExtensions
    {

        public static List<int> GetRoleIDs(this Translator @this) // will actually be a <UserRole> list
        {
            return @this.TranslatorRoles.Select(x => x.RoleID).ToList();
        }

        public static bool CanEditEmployees(this Translator @this)
        {
            return @this.IsManagement() || @this.IsHR();
        }


        public static List<Role> GetRoles(this Translator @this) // will actually be a <UserRole> list
        {
            return @this.TranslatorRoles.Select(x => x.Role).ToList();
        }

        public static bool IsInRole(this Translator @this, int roleId)
        {
            return @this.GetRoleIDs().Contains(roleId);
        }

        public static bool IsInRole(this Translator @this, Role role)
        {
            return @this.GetRoles().Contains(role);
        }

        public static CalendarPeriod GetCurrentCalendarPeriod(this Translator @this, DateTime currentDate)
        {
            var statuses = @this.CalendarPeriods;
            var result = GetCurrentCalendarRecord(statuses, currentDate);
            return result;
        }
        
        public static string GetCurrentCalendarStatus(this Translator @this, DateTime currentDate)
        {
            var statuses = @this.CalendarPeriods;
            var result = GetCurrentCalendarRecord(statuses, currentDate);
            return result == null ? "нет данных" : result.StaffStatus;
        }

        private static CalendarPeriod GetCurrentCalendarRecord(IEnumerable<CalendarPeriod> statuses, DateTime currentDate)
        {
            if (statuses.Count() == 0)
                return null;
            else
            {
                var recordedStatus = statuses.Where(x => x.StartDate <= currentDate.Date &&
                    (x.EndDate.HasValue && x.EndDate.Value.Date >= currentDate.Date)).LastOrDefault();
                if (recordedStatus == null)
                    return null;
                else
                    return recordedStatus;
            }
        }

        public static IQueryable<Translator> FilterByName(this IQueryable<Translator> @this, string name = "")
        {
            if (name != "")
                return @this.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            else 
                return @this;
        }

        public static IQueryable<Translator> FilterByOfficeId(this IQueryable<Translator> @this, int? officeId)
        {
            var id = officeId ?? -1;

            if (id > -1)
                return @this.Where(x => x.OfficeID == id);
            else 
                return @this;
        }

        public static IQueryable<Translator> Filter(this IQueryable<Translator> @this, EmployeeFilterCriteria criteria)
        {
            var acceptableStatuses = new List<string>();

            @this = @this.FilterByName(criteria.TxtName);

            if (String.IsNullOrEmpty(criteria.TxtName))
                @this = @this.FilterByOfficeId(criteria.DrdnOfficeID ?? -1);


            if (!criteria.ChkPresent ?? true) acceptableStatuses.Add("Я ");
            if (!criteria.ChkHoliday ?? true) acceptableStatuses.Add("В ");
            if (!criteria.ChkPaidOneDayHoliday ?? true) acceptableStatuses.Add("От");
            if (!criteria.ChkPaidHoliday ?? true) acceptableStatuses.Add("О ");
            if (!criteria.ChkUnpaidHoliday ?? true) acceptableStatuses.Add("Н ");
            if (!criteria.ChkSkip ?? true) acceptableStatuses.Add("П ");

            if (!criteria.ChkSick ?? true) acceptableStatuses.Add("Б ");
            if (!criteria.ChkBusinessTrip ?? true) acceptableStatuses.Add("К ");
            if (!criteria.ChkUnknown ?? true) acceptableStatuses.Add("");

            if (acceptableStatuses.Count > 0)
            {
                // TODO: construct this FUNC dynamically 
                var now = DateTime.Now;
                // abstract this !!! 
                Func<CalendarPeriod, bool> test = y =>
                    y.StartDate <= now &&
                    (y.EndDate.HasValue && y.EndDate.Value >= now) &&
                    acceptableStatuses.Contains(y.StaffStatus.ToUpper());

                @this = @this.Where(x => x.CalendarPeriods.Any(test));
            }

            if (criteria.ChkUnconfirmed ?? false)
            {
                var now = DateTime.Now;

                Func<CalendarPeriod, bool> test = y =>
                    y.StartDate <= now &&
                    (y.EndDate.HasValue && y.EndDate.Value >= now) &&
                    y.Confirmed;

                @this = @this.Where(x => x.CalendarPeriods.Where(test).Count() == 0);
            }

            return @this;
        }

    }
}
