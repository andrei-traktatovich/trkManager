using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic.TimeSheets;
using System.Collections.Generic;

namespace BusinessLogic.Tests.TimeSheets
{
    [TestClass]
    public class WorkDayTimeLineShould
    {
        private DateTime _startDate;
        private DateTime _endDate;

        private WorkDaySheetTimeLinePeriod _period1;
        private WorkDaySheetTimeLinePeriod _period2;
        private WorkDaySheetTimeLinePeriod _period3;
        private IEnumerable<WorkDaySheetTimeLinePeriod> _periods;
        private WorkDaysTimeLine _timeLine; 

        [TestInitialize]
        public void Setup()
        {
            _startDate = new DateTime(2014, 08, 25);
            _endDate = new DateTime(2014, 09, 25);

            _period1 = new WorkDaySheetTimeLinePeriod("period 1 (paid)", "office1", _startDate, _startDate.AddDays(2), true);
            _period2 = new WorkDaySheetTimeLinePeriod("period 2 (paid)", "office1", _startDate.AddDays(3), _startDate.AddDays(5), true);
            _period3 = new WorkDaySheetTimeLinePeriod("period 3 (unpaid)", "office1", _startDate.AddDays(6), _startDate.AddDays(8), false);

            _periods = new List<WorkDaySheetTimeLinePeriod>() 
            {
                _period1,
                _period2,
                _period3
            };

            _timeLine = new WorkDaysTimeLine("Иванов", "Переводчик", _startDate, _endDate);
            _timeLine.Add(_periods);
        }

        [TestMethod]
        public void BuildFromASingleEmployeeAndHisPeriods()
        {
            Assert.AreEqual(3, _timeLine.Count);
        }

        [TestMethod]
        public void ReturnDistinctPaidAndUnpaidDuration()
        {
            Assert.AreEqual(6, _timeLine.Duration);
            Assert.AreEqual(4, _timeLine.PaidDuration);
        }
    }
}
