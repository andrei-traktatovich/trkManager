using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketDataModel;

namespace BusinessLogic.Tests.TimeSheets
{
    public class MadaFaka
    {
        public DateTime StartDate = new DateTime(2014, 08, 25);
        public DateTime EndDate = new DateTime(2014, 09, 25);

        public Translator Ivanov = new Translator()
            {
                Name = "иванов",
                Title = new Title()
                {
                    Name = "Переводчик"
                }
            };
        public CalendarPeriod IvanovPeriod1;
        public CalendarPeriod IvanovPeriod2;
        public CalendarPeriod IvanovPeriod3;
        public Office Office1;

        public CalendarPeriod IvanovsStrayPeriod;
        public MadaFaka ()
	    {
            this.ExpectedDuration = 32;

            IvanovPeriod1 = new CalendarPeriod()
            {
                OfficeID = 1,
                StaffStatus = "Явка",
                StaffStatusEntity = new StaffStatus { IsPaid = true },
                StartDate = StartDate,
                EndDate = StartDate.AddDays(1),
                Translator = Ivanov
            };

            IvanovPeriod2 = new CalendarPeriod()
            {
                OfficeID = 1,
                StaffStatus = "Выходной",
                StaffStatusEntity = new StaffStatus { IsPaid = false },
                StartDate = StartDate.AddDays(2),
                EndDate = StartDate.AddDays(3),
                Translator = Ivanov
            };

            IvanovPeriod3 = new CalendarPeriod()
            {
                OfficeID = 1,
                StaffStatus = "Явка",
                StaffStatusEntity = new StaffStatus { IsPaid = true },
                StartDate = StartDate.AddDays(4),
                EndDate = StartDate.AddDays(5),
                Translator = Ivanov
            };

            IvanovsStrayPeriod = new CalendarPeriod()
            {
                OfficeID = 2,
                StaffStatus = "Явка",
                StaffStatusEntity = new StaffStatus { IsPaid = true },
                StartDate = StartDate.AddDays(6),
                EndDate = StartDate.AddDays(7),
                Translator = Ivanov
            };

            Ivanov.CalendarPeriods.Add(IvanovPeriod1);
            Ivanov.CalendarPeriods.Add(IvanovPeriod2);
            Ivanov.CalendarPeriods.Add(IvanovPeriod3);

            Office1 = new Office() 
            { 
                ID = 1, 
                Name = "office 1", 
                CalendarPeriods = { IvanovPeriod1, IvanovPeriod2, IvanovPeriod3 },
                Translators = { Ivanov }
            };
	    }

        public int ExpectedDuration { get; set; }
    }
}
