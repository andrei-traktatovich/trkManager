using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketDataModel
{
    public partial class Ticket
    {
        public const int Pending = 0;
        public const int InProgress = 1;
        public const int Declined = 2;
        public const int Closed = 3;
        public const int Confirmed = 4;
    }
}
