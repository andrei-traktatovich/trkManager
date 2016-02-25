using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic.TimeSheets;

namespace BusinessLogic.Tests.TimeSheets
{
    [TestClass]
    public class WorkDaySheetTimeLinePeriodShould
    {

        private DateTime _start = new DateTime(2014, 08, 25);
        private DateTime _end = new DateTime(2014, 08, 27);
        private WorkDaySheetTimeLinePeriod _period;

        [TestInitialize]
        [TestMethod]
        public void Setup()
        {
            _period = new WorkDaySheetTimeLinePeriod("test description", "test office", _start, _end, false); 
        }

        [TestMethod]
        public void ReturnDurationInDaysIfPaidAndDurationIsExclusive()
        {
            Assert.AreEqual(2, _period.Duration);
        }

        [TestMethod]
        public void Return0IfUnpaid()
        {
            Assert.AreEqual(0, _period.PaidDuration);
            Assert.AreEqual(2, _period.Duration);
        }
    }
}
