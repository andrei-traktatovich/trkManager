using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TicketDataModel;
using BusinessLogic.TimeSheets;

namespace TicketManager.Models.WorkDaySheet
{
    public class BreakupByOffice
    {
        public BreakupByOffice(BusinessLogic.TimeSheets.WorkDaySheet sheet)
        {
            _lines = new List<Line>();
            Build(sheet);
        }

        public string Name { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public IEnumerable<string> Offices { get; private set; }
        
        public IEnumerable<Line> Lines
        {
            get
            {
                return _lines.OrderBy(x => x.Name);
            }
        }

        public List<Line> _lines { get; set; }

        public class Line
        {
            public string Name { get; set;}
            public int Total{ get; set;}
            public Dictionary<string, int> Lines { get; set; }

        }

        private void Build(BusinessLogic.TimeSheets.WorkDaySheet sheet)
        {
            this.Name = sheet.Name;
            this.StartDate = sheet.StartDate;
            this.EndDate = sheet.EndDate;

            this.Offices = sheet.OfficeList.ToList();

            var existingOffices = sheet.TimeLines.Select(x => x.Periods.Select(y => y.OfficeName));



            foreach (var line in sheet.TimeLines)
                AddLine(line);
        }

        private void AddLine(BusinessLogic.TimeSheets.WorkDaysTimeLine line)
        {
            var newLine = new Line()
            {
                Name = line.Name,
                Total = line.PaidDuration
            };

            newLine.Lines = line.Periods.Where(x => x.IsPaid)
                .GroupBy(x => x.OfficeName)
                .Select(x => new { Office = x.Key, Length = x.Sum(y => y.PaidDuration) })
                .ToDictionary(
                    x => x.Office,
                    // I can't use this because I am using a string key, unfortunately ...
                    //x.Office.Length > 14 ? x.Office.Substring(0, 11) + "..." : x.Office,
                    x => x.Length
                    );

            this._lines.Add(newLine);
        }
    }
}