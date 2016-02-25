using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketDataModel
{
    public partial class Job
    {
        public int GetMargin(double? amount)
        {
            if (amount == 0 || !amount.HasValue)
                return 0;

            var totalParticipants = this.JobParticipants.Sum(x => (x.FinalAmount.HasValue ? x.FinalAmount : 0));

            var result = (int)((decimal)(amount.Value - totalParticipants) / (decimal)amount.Value * 100);
            return result;
        }

    }
}
