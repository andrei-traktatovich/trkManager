using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic.TimeSheets;
using TicketDataModel;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.TimeSheets
{
    [TestClass]
    public class WorkDaySheetShould
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private Office _office;
        private WorkDaySheet _sheet;
        private MadaFaka madaFaka = new MadaFaka();
        private int _expectedDuration;

        [TestInitialize]
        [TestMethod]
        [Ignore]
        public void Setup()
        {
            _startDate = madaFaka.StartDate;
            _endDate = madaFaka.EndDate;
            _expectedDuration = madaFaka.ExpectedDuration;
            _office = madaFaka.Office1; 

            //_sheet = new WorkDaySheet(_office, _startDate, _endDate);
            
            
        }

        [TestMethod]
        [Ignore]
        public void BuildFromOfficeAndItsEmployeesCalendarPeriods()
        {
            //_sheet.Build();
            //Assert.AreEqual(1, _sheet.Count);
        }

        [TestMethod]
        public void ComputeItsDuration()
        {
            //_sheet.Build();
            //Assert.AreEqual(_expectedDuration, _sheet.Duration);
        }

        [TestMethod]
        public void CountCorrectDurationOf3ForTimeLineOf3Days()
        {
            //_sheet.Build();
            //Assert.AreEqual(6, _sheet.TimeLines[0].Duration);
        }

        [TestMethod]
        public void CountCorrectPaidDurationOf2ForTimeLineOf3Days()
        {
            //_sheet.Build();
            //Assert.AreEqual(4, _sheet.TimeLines[0].PaidDuration);
        }

        [TestMethod]
        public void CountOnlyPeriodsFromThisOffice()
        {
            //AddStrayIvanov();
            //_sheet.Build(true);
            //Assert.AreEqual(3, _sheet.TimeLines[0].Periods.Count);
        }

        [TestMethod]
        public void CountPeriodsFromEveryWhere()
        {
            //AddStrayIvanov();
            //_sheet.Build(false);
            //Assert.AreEqual(4, _sheet.TimeLines[0].Periods.Count);// periods from all offices are counted;
        }

        private void AddStrayIvanov()
        {
             
            Translator ivanov = _office.Translators.First();
            ivanov.CalendarPeriods.Add(madaFaka.IvanovsStrayPeriod);
        }
    }
}
