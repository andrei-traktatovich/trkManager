using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketDataModel;

namespace BusinessLogic
{
    public class EventTracer
    {
        public class AppointmentEventTraceItem
        {
            public string Name { get; set; }
            public string JobName { get; set; }
            public string CreatedBy { get; set; }
            public DateTime StartDate { get; set;}
            public string StatusName { get; set; }
            public DateTime EndDate { get; set; }
            public string JobTypeName { get; set; }
        }

        private TraktatEntities _ctx;
        public EventTracer(TraktatEntities ctx)
        {
            _ctx = ctx;
        }



        
        public List<AppointmentEventTraceItem> GetRecentNewJobs(DateTime startDate)
        {
            var result = _ctx.JobParticipants.Where(x => x.Status > 0 &&
                x.ChangedDate.HasValue && x.ChangedDate.Value > startDate).OrderByDescending(x => x.ChangedDate.Value).Take(15).ToList();
            return result.Select(x => new AppointmentEventTraceItem
            {
                CreatedBy = x.CreatedBy,
                EndDate = x.EndDate.Value,
                StatusName = x.JobParticipantStatus.Name,
                JobTypeName = x.JobType.Name,
                StartDate = x.StartDate.Value,
                JobName = x.JobName,
                Name = x.Name
            })
            .ToList();
        }
    }
}
