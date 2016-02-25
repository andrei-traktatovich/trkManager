using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketDataModel
{
    public class EmployeeFilterCriteria
    {
        public string TxtName { get; set; }
        public int? DrdnOfficeID { get; set; }

        public bool? ShowReserve { get; set; }
        public bool? ShowBlackList { get; set; }
        public bool? ChkPresent { get; set; }
        public bool? ChkHoliday { get; set; }
        public bool? ChkPaidOneDayHoliday { get; set; }
        public bool? ChkPaidHoliday { get; set; }
        public bool? ChkUnpaidHoliday { get; set; }
        public bool? ChkSkip { get; set; }
        public bool? ChkSick { get; set; }
        public bool? ChkBusinessTrip { get; set; }
        public bool? ChkUnconfirmed { get; set; }
        public bool? ChkUnknown { get; set; }
    }
}
