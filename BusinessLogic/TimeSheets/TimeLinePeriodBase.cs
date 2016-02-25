using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.TimeSheets
{
    public class TimeLinePeriodBase
    {
        public string Text { get; internal set; }
        public DateTime Start { get; internal set; }
        public DateTime End { get; internal set; }
        public TimeLinePeriodBase(string text, DateTime start, DateTime end)
        {
            Text = text;
            if (start == null)
                throw new ArgumentNullException("start", "TimeLinePeriod: start cannot be null");
            if (end == null)
                throw new ArgumentNullException("end", "TimeLinePeriod: end cannot be null");

            if (end < start)
            {
                var t = start;
                start = end;
                end = t;
            }
                
            Start = start;
            End = end;
            
        }
 
    }
}
