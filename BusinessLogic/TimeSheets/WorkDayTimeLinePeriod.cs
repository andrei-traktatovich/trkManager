using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.TimeSheets
{
    public class WorkDaySheetTimeLinePeriod : TimeLinePeriodBase
    {
        public bool IsPaid { get; private set; }

        public int Duration
        {
            get { return (int)(this.End - this.Start).TotalDays; }
        }

        public string OfficeName { get; private set; }

        public int PaidDuration
        {
            get
            {
                return this.IsPaid ? this.Duration : 0;
            }
        }

        public WorkDaySheetTimeLinePeriod(string status, string officeName, DateTime start, DateTime end, bool isPaid) :
            base(status, start, end)
        {
            this.IsPaid = isPaid;
            this.OfficeName = officeName; 
        }

        internal bool ImmediatelyPrecedes(WorkDaySheetTimeLinePeriod next)
        {
            return (next.Start.Date == this.End.Date || next.Start.Date == this.End.Date.AddDays(1));
        }

        internal bool IsSameAs(WorkDaySheetTimeLinePeriod comparandum)
        {
            return (this.IsPaid == comparandum.IsPaid 
                && this.Text == comparandum.Text
                && this.OfficeName == comparandum.OfficeName);
        }
    }
}
