using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;

namespace TicketManager
{
    public class PaymentGroup
    {
        #region constructors

        public PaymentGroup(TraktatEntities ctx, string jobID)
        {
            CreatePaymentGroup(ctx, jobID);
        }

        public PaymentGroup(TraktatEntities ctx, Job job)
        {
            CreatePaymentGroup(ctx, job.AccountingRecordID);
        }

        #endregion 

        private void CreatePaymentGroup(TraktatEntities ctx, string groupID)
        {
            _jobs = ctx.Jobs.Where(x => x.AccountingRecordID == groupID).ToList();
            _paymentDocs = ctx.PaymentInfos.Include("Payments").Where(x => x.JobGroupID == groupID).ToList();
        }

        private List<Job> _jobs;

        private List<PaymentInfo> _paymentDocs;
    }

}
