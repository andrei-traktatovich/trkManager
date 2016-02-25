using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;

namespace BusinessLogic
{
    public static class EmployeeExt
    {
        public static Office GetReceiptOffice(this Translator employee, DateTime referenceDate)
        {
            var d = referenceDate.LastMonthDay();
            var d1 = referenceDate.AddDays(1);
            var period = employee.CalendarPeriods.Where(x => x.StartDate.Value <= d && x.EndDate.Value > d1).FirstOrDefault();
            if (period != null)
                return period.Office;
            else
                return null;
        }
    }
}
