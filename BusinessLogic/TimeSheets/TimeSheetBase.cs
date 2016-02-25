using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.TimeSheets
{
    
    public class TimeSheetBase<TLine, TPeriod> 
        where TLine : TimeLineBase<TPeriod>
        where TPeriod : TimeLinePeriodBase
    {
        private string _name;
        private DateTime _startDate;
        private DateTime _endDate;
        private List<TLine> _timeLines = new List<TLine>();

        public TimeSheetBase(string name, DateTime start, DateTime? end = null)
        {
            this._name = name;
            if (start == null)
                throw new ArgumentNullException("startDate", "param cannot be null");
 
            this._startDate = start.Date;

            this._endDate = (end ?? start.AddMonths(1)).Date.AddDays(1); // INCLUSIVE!! 

            if (_startDate > _endDate)
                throw new ArgumentException("startDate cannot be later than endDate");
            if (_startDate.Equals(_endDate))
                throw new ArgumentException("startDate cannot be equal to endDate");
        }

        public virtual string Name { get { return _name; } }

        public virtual DateTime StartDate { get { return _startDate; } }

        public virtual DateTime EndDate { get { return _endDate; } }

        public List<TLine> TimeLines { get { return _timeLines; } }

        public TLine AddTimeLine(TLine line)
        {
            _timeLines.Add(line);

            return line;
        }

        public int Count { get { return _timeLines.Count; } }
    }

}


 