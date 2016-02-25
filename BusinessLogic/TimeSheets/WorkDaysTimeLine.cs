using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.TimeSheets
{
    public class WorkDaysTimeLine : TimeLineBase<WorkDaySheetTimeLinePeriod>
    {
        public WorkDaysTimeLine(string name, string description, DateTime startDate, DateTime endDate) 
            : base(name, description, startDate, endDate)
        {
        }

        public int Duration
        {
            get
            {
                if (this.Periods.Count == 0)
                    return 0;
                else
                    return this.Periods.Sum(x => x.Duration);
            }
        }

        public int PaidDuration
        {
            get
            {
                if (this.Periods.Count == 0)
                    return 0;
                else
                    return this.Periods.Sum(x => x.PaidDuration);
            }
        }
        public override void Add(IEnumerable<WorkDaySheetTimeLinePeriod> timeLinePeriods)
        {
            if (timeLinePeriods == null || timeLinePeriods.Count() == 0)
                return;

            var periods = MergeAdjacent(timeLinePeriods);
            base.Add(periods);
        }

        private IEnumerable<WorkDaySheetTimeLinePeriod> MergeAdjacent(IEnumerable<WorkDaySheetTimeLinePeriod>  timeLinePeriods)
        {
            if (timeLinePeriods.Count() == 1)
                return timeLinePeriods;

            var periods = timeLinePeriods.OrderBy(x => x.Start).ToList();
            for (var i = periods.Count - 1; i > 0; i--)
            {
                var isNeighbor = periods[i - 1].ImmediatelyPrecedes(periods[i]);
                var areSame = periods[i - 1].IsSameAs(periods[i]);
                if (isNeighbor && areSame)
                {
                    periods[i - 1].End = periods[i].End;
                    periods.RemoveAt(i);
                }
            }

            return periods;
        }


        public string ReceiptOfficeName { get; set; }
    }
}
