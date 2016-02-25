using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;


namespace BusinessLogic
{
    public class StatusAndBusyReport
    {
        private TraktatEntities _ctx;
        public StatusAndBusyReport (TraktatEntities ctx)
	    {
            _ctx = ctx;
	    }

        public IList<ContactAndStatusChangeReportItem> Create(int officeID, string jobName, DateTime start, DateTime end)
        {
            var result = _ctx.ContactAndStatusChangeReportItems.AsQueryable();
            
            if (officeID > -1)
            {
                var office = _ctx.Offices.Where(x => x.ID == officeID).SingleOrDefault();
                if (office != null)
                {
                    var name = office.Name;
                    result = result.Where(x => x.Office.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            if (!string.IsNullOrEmpty(jobName))
            {
                result = result.Where(x => x.JobName.Contains(jobName));
            }
            end = end.Date.AddDays(1);
            var list = result.Where(x => x.ContactDate >= start && x.ContactDate < end).OrderBy(x=>x.ContactDate)
                .ToList();
            return list;
        }
    }
}
