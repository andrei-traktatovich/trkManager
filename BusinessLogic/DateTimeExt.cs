using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public static class DateTimeExt
    {
        public static bool IsDateEarlierThan(this DateTime startDate, DateTime endDate)
        {
            return startDate.Date < endDate.Date;
        }
    }
}
