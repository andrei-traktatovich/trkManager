using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;

namespace BusinessLogic.TimeSheets
{
    public class WorkDaySheet : TimeSheetBase<WorkDaysTimeLine, WorkDaySheetTimeLinePeriod>
    {
        public WorkDaySheet(string name, DateTime start, DateTime? end = null)  : base(name, start, end)
        {
            
        }

        public IEnumerable<string> OfficeList
        {
            get { return this.TimeLines.Select(x => x.Periods.Select(y => y.OfficeName)).SelectMany(x => x).Distinct(); }
        }
        
        public static WorkDaySheet Build(IQueryable<CalendarPeriod> data, DateTime start, DateTime end, string name) 
        {
            var result = new WorkDaySheet(name, start, end);
            
            var grouped = data.ByEmployee();

            foreach (var item in grouped)
            {
                var timeLine = new WorkDaysTimeLine(item.Key.Name, item.Key.Title.Name, start, end);
                var periods = item
                    .Select(x => new WorkDaySheetTimeLinePeriod(
                        status: x.StaffStatus,
                        officeName: x.Office.Name,
                        start: x.StartDate.Value,
                        end: x.EndDate.Value,
                        isPaid: x.StaffStatusEntity.IsPaid))
                    .OrderBy(x => x.Start)
                    .ToArray();
                timeLine.Add(periods);
                result.AddTimeLine(timeLine);
            }

            return result;
        }
        public int Duration
        {
            get
            {
                return (int)(this.EndDate - this.StartDate).TotalDays;
            }
        }
    }
}
