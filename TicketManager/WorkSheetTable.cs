using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TicketDataModel;

namespace TicketManager
{
    public class WorkSheetTable
    {
        public class WorkSheetLine
        {
            public Translator Employee { get; private set;  }
            public List<CalendarInfo> Days { get; private set;  }
            public int TotalWorkingDays { get; private set; }
            

            public WorkSheetLine(Translator employee)
            {
                Employee = employee;
            }

            public void Calculate(TraktatEntities ctx, Calendar calendar, DateTime startDate, DateTime endDate)
            {
                this.Days = calendar.GetCalendar(Employee, startDate, endDate).ToList();
                TotalWorkingDays = 0;
                
                foreach (var day in Days)
                {
                     
                    if (day.Confirmed)
                    {
                        if (day.Status.IsPaid)  
                            TotalWorkingDays++;
                    }
                }
            }
        }


        private TraktatEntities _ctx;
        private int _officeID;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int DaysCount { get; private set;}
        private Translator CurrentUser;
        public WorkSheetTable(int officeID, TraktatEntities ctx, Translator currentUser)
        {
            _officeID = officeID;
            _ctx = ctx;
            CurrentUser = currentUser;
        }

        public void Build(DateTime startDate, DateTime endDate)
        {
            var result = new List<WorkSheetLine>();
            StartDate = startDate.Date;
            EndDate = endDate.Date;
            // getting employees from this office
            var employees = GetEmployees(_ctx, _officeID, startDate, endDate).OrderBy(x => x.Name); 
            if (employees == null || employees.Count() == 0)
                return;

            var calendar = new Calendar(_ctx, CurrentUser);
            // creating a line per each employee
            foreach (var employee in employees)
            {
                var workSheetLine = new WorkSheetLine(employee);
                workSheetLine.Calculate(_ctx, calendar, startDate, endDate);
                result.Add(workSheetLine);
            }
            
            this.Lines = result;
        }

        public List<WorkSheetLine> Lines { get; private set; }

        private IEnumerable<Translator> GetEmployees(TraktatEntities ctx, int officeID, DateTime startDate, DateTime endDate)
        {
            return ctx.Translators.Where(x => (x.OfficeID == officeID || x.Podr_now == officeID)
                && (x.active.ToLower() == "новый" || x.active.ToLower() == "активный")
                && x.TitleStatusID == 1
                && x.CreationDate <= endDate);
        }

        internal object ToExcel()
        {
            throw new NotImplementedException();
        }
    }
}