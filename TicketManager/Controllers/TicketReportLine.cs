using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketDataModel;

namespace TicketManager.Controllers
{
    public class TicketReportLine
    {
        public TicketReportLine(IGrouping<int, Ticket> group)
        {
            children = new List<TicketDescription>();
            foreach (Ticket t in group)
            {
                var td = new TicketDescription(t);
                children.Add(td);
            }
        ResponseTime = AverageResponseTime(group.ToList());
        Count = children.Count.ToString();
        Level = group.First().TicketLevel.Name;
        }
        public string id { get; set; }
        public string Level { get; set; }
        public string ResponseTime { get; set;}
        public string Count { get; set;}
        public string Type { get; set; }
        public string KeyWords { get; set; }
        public string Office { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string LastChangedDate { get; set; }
        public List<TicketDescription> children { get; set;}

        public string AverageResponseTime(List<Ticket> tickets)
        {
            double responsetimes = 0;
            int counter = 0;
            foreach (var t in tickets)
            {
                if (t.ChangedDate.HasValue && t.CreatedDate.HasValue) // t.StatusID >= Ticket.Closed && 
                {
                    var time = t.ChangedDate.Value.Subtract(t.CreatedDate.Value).TotalMinutes;
                    responsetimes += time;
                    counter++;
                }
            }
            var averageresult = Math.Round(responsetimes / counter, 0);

            return averageresult.ToString();
        }

    }

    public class TicketDescription
    {
        public TicketDescription(Ticket ticket)
        {
            id = ticket.ID.ToString();
            Type = ticket.TicketType.Name;
            KeyWords = ticket.KeyWords;
            Office = ticket.AssigneeOffice.Name;
            Status = ticket.TicketStatus.Name;
            CreatedDate = ticket.CreatedDate.HasValue ? ticket.CreatedDate.Value.ToString("hh.mm dd.MM.yy") : "";
            LastChangedDate = ticket.ChangedDate.HasValue ? ticket.ChangedDate.Value.ToString("hh.mm dd.MM.yy") : "";
            if (CreatedDate != "" && LastChangedDate != "")
            {
                ResponseTime = Math.Round(ticket.ChangedDate.Value.Subtract(ticket.CreatedDate.Value).TotalMinutes, 0).ToString();
            }
            else
                ResponseTime = "???";
        }
        public string id;
        public string Type { get; set; }
        public string KeyWords { get; set; }
        public string Office { get; set; }
        public string Status { get; set; }
        public string CreatedDate { get; set; }
        public string LastChangedDate { get; set; }
        public string ResponseTime { get; set; }
        public string Level = "";
        public string Count = "";
    }

}
