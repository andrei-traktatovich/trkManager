using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic.TimeSheets;
using System.Collections.Generic;

namespace BusinessLogic.Tests.TimeSheets
{
    [TestClass]
    public class TimeSheetBaseShould
    {
        private TimeSheetBase<TimeLineBase<TimeLinePeriodBase>, TimeLinePeriodBase>  _sheet;

        [TestInitialize]
        public void Setup()
        {
            _sheet = new TimeSheetBase<TimeLineBase<TimeLinePeriodBase>, TimeLinePeriodBase>("test", DateTime.Now, DateTime.Now.AddDays(30));
        }

        [TestMethod]
        public void RoundDateTimeofStartDate()
        {
            var testStartDate = DateTime.Now.AddMinutes(15).AddSeconds(30);
            var testEndDate = DateTime.Now.AddDays(20).AddMinutes(15).AddSeconds(30);

            var sheet = new TimeSheetBase<TimeLineBase<TimeLinePeriodBase>, TimeLinePeriodBase>("test", testStartDate);
            Assert.AreEqual(testStartDate.Date, sheet.StartDate);
            //Assert.AreEqual(testEndDate.Date, sheet.EndDate);
        }

        [TestMethod]
        public void RoundDateTimeOfEndDate()
        {
            var testStartDate = DateTime.Now.AddMinutes(15).AddSeconds(30);
            var testEndDate = DateTime.Now.AddDays(20).AddMinutes(15).AddSeconds(30);

            var sheet = new TimeSheetBase<TimeLineBase<TimeLinePeriodBase>, TimeLinePeriodBase>("test", testStartDate, testEndDate);
            Assert.AreEqual(testEndDate.Date, sheet.EndDate);
        }

        [TestMethod]
        public void ByDefaultCreateAMonthPeriod()
        {
            var testStartDate = new DateTime(2014, 08, 25).Date;
            var testEndDate = new DateTime(2014, 09, 25).Date;
            var sheet = new TimeSheetBase<TimeLineBase<TimeLinePeriodBase>, TimeLinePeriodBase>("test", testStartDate);
            Assert.AreEqual(testEndDate, sheet.EndDate);
        }

        [TestMethod]
        public void ConvertASeriesOfDateTimeIntoALine()
        {
            var name = "test";
            var description = "test Description";
            var line = new TimeLineBase<TimeLinePeriodBase>(name, description, DateTime.Now, DateTime.Now.AddDays(20));
            var timeLine = _sheet.AddTimeLine(line);
            Assert.IsTrue(_sheet.Count == 1);
        }
    }
}
     
     