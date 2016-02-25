using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic.TimeSheets;

namespace BusinessLogic.Tests.TimeSheets
{
    [TestClass]
    public class TimeLineShould
    {
        private DateTime _startDate;
        private DateTime _endDate;
        
        private TimeLineBase<TimeLinePeriodBase> _timeLine;

        [TestInitialize]
        [TestMethod]
        public void Setup()
        {
            _startDate = new DateTime(2014, 08, 25).Date;
            _endDate = new DateTime(2014, 09, 25).Date;

            _timeLine = new TimeLineBase<TimeLinePeriodBase>("test", "test description", _startDate, _endDate);

        }

        [TestMethod]
        public void KeepDatesIntactIfInRange()
        {
            _timeLine.Add(new TimeLinePeriodBase("test", _startDate.AddDays(1), _endDate.AddDays(-1)));
            Assert.AreEqual(1, _timeLine.Count);
        }

        [TestMethod]
        public void AdjustPastBoundaryIfOutOfRange()
        {
            _timeLine.Add(new TimeLinePeriodBase("test", _startDate.AddDays(-1), _endDate.AddDays(-1)));
            Assert.AreEqual(1, _timeLine.Count);
            Assert.AreEqual(_startDate, _timeLine.Periods[0].Start);
        }

        [TestMethod]
        public void AdjustFutureBoundaryIfOutOfRange()
        {
            _timeLine.Add(new TimeLinePeriodBase("test", _startDate.AddDays(1), _endDate.AddDays(2)));
            Assert.AreEqual(1, _timeLine.Count);
            Assert.AreEqual(_endDate, _timeLine.Periods[0].End);
        }

        [TestMethod]
        public void IgnoreIfInFuture()
        {
            _timeLine.Add(new TimeLinePeriodBase("test", _startDate.AddDays(100), _endDate.AddDays(120)));
            Assert.AreEqual(0, _timeLine.Count);
        }

        [TestMethod]
        public void IgnoreIfInPast()
        {
            _timeLine.Add(new TimeLinePeriodBase("test", _startDate.AddDays(-120), _endDate.AddDays(-100)));
            Assert.AreEqual(0, _timeLine.Count);
        }
    }
    
}
