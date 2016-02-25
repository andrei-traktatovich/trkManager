using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;

namespace BusinessLogic
{
    public class CurrentJobsSchedule
    {
        private TraktatEntities _ctx; 

        public CurrentJobsSchedule(TraktatEntities ctx)
        {
            _ctx = ctx;
        }

        public class JobScheduleItem
        {
            public string Name { get; set; }
            public string JobInfo { get; set;}
            public DateTime StartDate { get; set;}
            public DateTime EndDate { get; set;}
        }

        public List<JobScheduleItem> GetJobScheduleItems(DateTime startDate, DateTime endDate)
        {
            var result = new List<JobScheduleItem>();
            var candidates = _ctx.JobParticipants
                .Where(x => x.EndDate.HasValue && x.EndDate.Value >= startDate &&
                    x.Status.HasValue && x.Status.Value > 0 && 
                    x.StartDate.HasValue && x.StartDate <= endDate)
                .ToList();

            var candidatesWithOtherStatuses = _ctx.Translators.Where(x => x.CurrentStatusID.HasValue && x.CurrentStatusID.Value > 2).ToList();

            var otherStatuses = candidatesWithOtherStatuses
                .Select(x => new JobScheduleItem
                {
                    JobInfo = x.CurrentStatus.Name + (x.StatusComment ?? ""),
                    Name = x.Name,
                    StartDate = startDate,
                    EndDate = (x.StatusValidThrough.GetValueOrDefault() > endDate ? endDate : x.StatusValidThrough.GetValueOrDefault() > startDate ? x.StatusValidThrough.GetValueOrDefault() : startDate)
                }).ToList();

            result = candidates
                .Select(x => new JobScheduleItem
                {
                    StartDate = x.StartDate.Value < startDate ? startDate : x.StartDate.Value,
                    EndDate = x.EndDate.Value > endDate ? endDate : x.EndDate.Value,
                    Name = x.Name,
                    JobInfo = string.Format("{0} ({1})", x.JobName, x.JobParticipantStatus.Name)
                })
                .Union(otherStatuses)
                .OrderBy(x => x.Name)
                .ToList();

            return result;
        }
    }
}
