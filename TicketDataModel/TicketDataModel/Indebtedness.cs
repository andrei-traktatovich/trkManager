using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketDataModel
{
    public class Indebtedness
    {
        TraktatEntities ctx = new TraktatEntities();

        public struct IndebtedCustomer
        {
            public Customer Customer;
            public PaymentInfo Invoice;
            public List<Job> Jobs;
        }

        public List<IndebtedCustomer> IndebtedCustomerData { get; private set; }

        public Indebtedness(string jobID)
        {
            IndebtedCustomerData = new List<IndebtedCustomer>();
            var job = ctx.Jobs.Find(jobID);
            if (job == null)
                throw new ArgumentException();
            var groupID = job.AccountingRecordID;
            var invoices = ctx.PaymentInfos.Where(x => x.JobGroupID == groupID);
            if (invoices.Count() > 0)
            {
                var jobs = ctx.Jobs.Where(x => x.AccountingRecordID == groupID).ToList();
                var customer = job.Customer;
                foreach (var invoice in invoices)
                {
                    var indebtedCustomer = new IndebtedCustomer { Customer = customer, Invoice = invoice, Jobs = jobs };
                    IndebtedCustomerData.Add(indebtedCustomer);
                }
            }
        }

        public Indebtedness(DateTime startDate, DateTime endDate, bool onlyIndebted, int? officeID)
        {
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1);

            IndebtedCustomerData = new List<IndebtedCustomer>();
            var UnpaidInvoices = ctx.PaymentInfos
                .Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate);

            if (onlyIndebted)
                UnpaidInvoices = UnpaidInvoices.Where(x => (!x.PaymentAmount.HasValue || x.PaymentAmount.Value < x.AmountDue.Value));

            if (officeID.HasValue && officeID.Value >= 00)
                UnpaidInvoices = UnpaidInvoices.Where(x => x.OfficeID == officeID);

            foreach (var invoice in UnpaidInvoices)
            {
                var customers = ctx.Customers.Where(x => x.Jobs.Any(y => y.AccountingRecordID == invoice.JobGroupID));
                var customer = customers.Distinct().SingleOrDefault();
                
                if (customer == null)
                    continue;
                var jobs = ctx.Jobs.Where(x => x.AccountingRecordID == invoice.JobGroupID).ToList();
                var indebtedCustomer = new IndebtedCustomer() { Customer = customer, Invoice = invoice, Jobs = jobs };
                IndebtedCustomerData.Add(indebtedCustomer);
            }
        }

        public List<IGrouping<Customer, IndebtedCustomer>> GetGrouping()
        {
            return IndebtedCustomerData.GroupBy(x => x.Customer).ToList();
        }
    }
}
