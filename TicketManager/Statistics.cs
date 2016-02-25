using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicketManager
{
    public class StatisticsWithTotals
    {
        public List<SimpleStatisticsData> Lines
        {
            get { return _lines.OrderBy(x => x.Key).ToList(); }
        }
        private List<SimpleStatisticsData> _lines = new List<SimpleStatisticsData>();

        public void Add(IGrouping<string, TicketDataModel.Job> jobs)
        {
            if (jobs == null || jobs.Key == null)
                return;

            var s = AddLine(jobs.Key,
                jobs.Sum(x => (double)(x.PagesCountWithSpaces.HasValue ? x.PagesCountWithSpaces.Value : 0)),
                jobs.Sum(x => (double)(x.AmountAtJobCreate.HasValue ? x.AmountAtJobCreate.Value : 0)),
                jobs.Sum(x => (double)(x.AmountFinal ?? 0)),
                jobs.Sum(x => (double)(x.FinalPagesCountForCustomer ?? 0)),
                jobs.Sum(x => (double)(x.DiscountedAmount ?? 0)),
                jobs.Count());

            if (jobs != null)
                s.Jobs = jobs.GroupBy(x => x.Customer.Name).ToList();
        }

        private SimpleStatisticsData AddLine(string jobType, double? initialPages, double? initialAmount, double? finalAmount, double? finalPages, double? finalAmountWithDiscount, int jobsCount)
        {
            var s = new SimpleStatisticsData
            {
                Key = jobType,
                FinalAmount = finalAmount ?? 0,
                FinalAmountWithDiscount = finalAmountWithDiscount ?? 0,
                FinalPageCount = finalPages ?? 0,
                InitialAmount = initialAmount ?? 0,
                InitialPageCount = initialPages ?? 0,
                JobsCount = jobsCount
            };

            _lines.Add(s);
            _totalJobsCount += jobsCount;
            _totalFinalAmount += (finalAmount ?? 0);
            _totalFinalAmountWithDiscount += (finalAmountWithDiscount ?? 0);
            _totalFinalPageCount += (finalPages ?? 0);
            _totalInitialAmount += (initialAmount ?? 0);
            _totalInitialPageCount += (initialPages ?? 0);
            
            
            return s;
        }

        private int _totalJobsCount;
        private double _totalInitialPageCount;
        private double _totalFinalAmount;
        private double _totalInitialAmount;
        private double _totalFinalPageCount;
        private double _totalFinalAmountWithDiscount;
        

        public int TotalJobsCount
        {
            get { return _totalJobsCount; }
        }
        public double TotalInitialAmount
        {
            get { return _totalInitialAmount; }
        }

        public double TotalFinalAmount
        {
            get { return _totalFinalAmount; }
        }

        public double TotalInitialPageCount
        {
            get { return _totalInitialPageCount; }
        }
        public double TotalFinalPageCount
        {
            get { return _totalFinalPageCount; }
        }
        public double TotalFinalAmountWithDiscount
        {
            get { return _totalFinalAmountWithDiscount; }

        }

    }
    public class SimpleStatisticsData
    {
        public string Key;
        public int JobsCount;
        public double InitialAmount;
        public double InitialPageCount;
        public double FinalAmount;
        public double FinalPageCount;
        public double FinalAmountWithDiscount;
        public List<IGrouping<string, TicketDataModel.Job>> Jobs;
        public double Paid;
    }

}