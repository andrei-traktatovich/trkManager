using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TicketDataModel;

namespace TicketManager
{
    public class ParticipantData
    {
        public string Id;
        public string Name;
        public string DateStart;
        public string DateEnd;
        public string JobType;
        public List<ParticipantData> children = new List<ParticipantData>();

        public ParticipantData(JobParticipant participant)
        {
            if (participant == null)
                throw new ArgumentNullException();
            this.Id = participant.ID;
            this.Name = participant.Name;
            this.DateStart = participant.StartDate.ToString();
            this.DateEnd = participant.EndDate.ToString();
            this.JobType = participant.JobType.Name;
        }
    }
    
}