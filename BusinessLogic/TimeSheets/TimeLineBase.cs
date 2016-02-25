using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.TimeSheets
{
    public class TimeLineBase<T> 
        where T : TimeLinePeriodBase 
    {
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public string Description { get; private set; }
        public string Name { get; private set; }
        public List<T> Periods { get; set; }

        public TimeLineBase(string name, string description, DateTime start, DateTime end)  
        {
            StartDate = start;
            EndDate = end;
            Name = name;
            Description = description;
            Periods = new List<T>();
        }

        public void Add(T timeLinePeriod)
        {
            TryAdd(timeLinePeriod);
        }

        public virtual void Add(IEnumerable<T> timeLinePeriods)
        {
            foreach (var t in timeLinePeriods)
                Add(t);
        }

        public bool TryAdd(T timeLinePeriod)
        {
            var success = TryAdjust(timeLinePeriod);
            
            if (success)
                this.Periods.Add(timeLinePeriod);
            
            return success;
        }

        public bool TryAdjust(TimeLinePeriodBase period)
        {
            if (period.Start > this.EndDate)
                return false;

            if (period.End < this.StartDate)
                return false;

            var start = period.Start >= this.StartDate ? period.Start : this.StartDate;

            var end = period.End <= this.EndDate ? period.End : this.EndDate;

            if (start < end)
            {
                period.Start = start;
                period.End = end;
                return true;
            }

            return false;
        }

        public int Count { get { return Periods.Count; } }
    }

}
