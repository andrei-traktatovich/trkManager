using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketDataModel;

namespace TicketManager.Controllers
{
    [Authorize]
    [TraktatHandleError]
    public class StatisticsController : ADController
    {
        //
        // GET: /Statistics/

        TraktatEntities ctx = new TraktatEntities();

        public ActionResult Index()
        {
            //var offices = ctx.Offices.Where(x => x.pod_rod == 1 && x.Del == 0).OrderBy(x => x.Name);
            //List<SelectListItem> officesSelectList = new SelectList(offices, "ID", "Name").ToList();
            
            //officesSelectList.Insert(0, new SelectListItem { Text = "Все", Value = "-1", Selected = false });
            var officesSelectList = this.GetOfficesDrDn();
            ViewData["officesList"] = officesSelectList;
            
            var dateOptions = new List<SelectListItem> { 
                new SelectListItem { Value = "0", Text = "Дата получения", Selected = true }, 
                new SelectListItem { Value = "1", Text = "Дата факт.сдачи" },
                new SelectListItem { Value = "2", Text = "Дата оплаты" }
            };

            ViewData["dateOptions"] = dateOptions;
            
            return View();
        }

        // will need to refactor this code
        public ActionResult DisplayStatisticsByOffice(DateTime startDate, DateTime endDate, int startDateOption, int endDateOption, bool includeSubJobs)
        {
            Func<IEnumerable<Job>, IEnumerable<IGrouping<string, Job>>> groupingFn = (x => x.GroupBy(y => y.Office.Name));

            var statistics = GetStatistics(-1, startDate.Date, endDate.Date.AddDays(1), startDateOption, endDateOption, groupingFn, includeSubJobs);

            var title = string.Format("Статистика по офисам за период с {0} по {1}", startDate.Date.ToString("dd.MM.yyyy"), endDate.Date.ToString("dd.MM.yyyy"));
            ViewBag.Title = title;
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;
            ViewData["officeID"] = -1;
            return PartialView(statistics);
        }

        public ActionResult DisplaySimpleStatistics(int officeID, DateTime startDate, DateTime endDate, int startDateOption, int endDateOption, bool includeSubJobs)
        {

            Func<IEnumerable<Job>, IEnumerable<IGrouping<string, Job>>> groupingFn = (x => x.GroupBy(y => y.JobType.Name));

            var statistics = GetStatistics(officeID, startDate.Date, endDate.Date.AddDays(1), startDateOption, endDateOption, groupingFn, includeSubJobs);
            string officeName = officeID == -1 ? "Все" : ctx.Offices.Where(x => x.ID == officeID).FirstOrDefault().Name;
            var title = string.Format("Статистика филиала: {0} за период с {1} по {2}", officeName, startDate.Date.ToString("dd.MM.yyyy"), endDate.Date.ToString("dd.MM.yyyy"));
            ViewBag.Title = title;
            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;
            ViewData["officeID"] = officeID;
            return PartialView(statistics);
        }

        public ActionResult DisplaySimpleStatisticsByPeriods(int? officeID, DateTime? startDate, DateTime? endDate)
        {
            startDate = startDate ?? new DateTime(DateTime.Now.Year,1,1);
            endDate = endDate ?? DateTime.Now.Date;
            officeID = officeID ?? -1;
            var statistics = GetStatisticsByPeriods(officeID.Value, startDate.Value, endDate.Value);
            return PartialView(statistics);
        }

        public List<SimpleStatisticsData> GetStatisticsByPeriods(int officeID, DateTime startDate, DateTime endDate)
        {
            // getting how many month periods there are
            var result = new List<SimpleStatisticsData>();
            
            
            var ctx = new TraktatEntities();
            for (var counter = startDate; counter < endDate; counter = counter.AddMonths(1))
            {
                var rightLimit = counter.AddMonths(1);
                var byMonth = ctx.Jobs
                    .Where(x => x.JobStatusID.HasValue && x.JobStatusID.Value  > 0 && x.TakeUpDate.Value >= counter && x.TakeUpDate.Value <= rightLimit).ToList();
                var line = new SimpleStatisticsData()
                {
                    FinalAmount = byMonth.Sum(x => (x.AmountFinal ?? 0)),
                    FinalAmountWithDiscount = byMonth.Sum(x => (x.DiscountedAmount ?? 0)),
                    FinalPageCount = byMonth.Sum(x => (x.FinalPagesCountWithSpaces ?? 0)),
                    InitialAmount = byMonth.Sum(x => (x.AmountAtJobCreate ?? 0)),
                    InitialPageCount = byMonth.Sum(x => (x.PagesCountWithSpaces ?? 0)),
                    JobsCount = byMonth.Count()
                };
                result.Add(line);
                
            }
            return result;
        }

        public Func<IEnumerable<Job>, IEnumerable<Job>> GetStartFilterFunc(int option, DateTime startDate)
        {
            switch (option)
            {
                case 0: return (x => x.Where(j => j.TakeUpDate >= startDate));  
                case 1: return (x => x.Where(j => j.SentToCustomerDate.HasValue && j.SentToCustomerDate.Value >= startDate));  
                case 2: return (x => x.Where(j => j.InvoicePaidDate.HasValue && j.InvoicePaidDate.Value >= startDate));  
            }
            throw new ArgumentException();
        }
        public Func<IEnumerable<Job>, IEnumerable<Job>> GetEndFilterFunc(int option, DateTime endDate)
        {
            switch (option)
            {
                case 0: return (x => x.Where(j => j.TakeUpDate <= endDate));
                case 1: return (x => x.Where(j => j.SentToCustomerDate.HasValue && j.SentToCustomerDate.Value <= endDate));
                case 2: return (x => x.Where(j => j.InvoicePaidDate.HasValue && j.InvoicePaidDate.Value <= endDate));
            } throw new ArgumentException();
        }

        public StatisticsWithTotals GetStatistics(int officeID, DateTime startDate, DateTime endDate, int startDateOption, int endDateOption,
            Func<IEnumerable<Job>, IEnumerable<IGrouping<string, Job>>> groupingFn, bool includeSubJobs)
        {

            
            // different filtering options 
            Func<IEnumerable<Job>, IEnumerable<Job>> startFilterFunc = GetStartFilterFunc(startDateOption, startDate);
            Func<IEnumerable<Job>, IEnumerable<Job>> endFilterFunc = GetEndFilterFunc(endDateOption, endDate);

            IQueryable<TicketDataModel.Job> filteredJobs;
            if (officeID > -1)
            {
                filteredJobs = ctx.Jobs.Where(x => x.OfficeID == officeID);
            }
            else
                filteredJobs = ctx.Jobs;
            
            // do we include subjobs? 
            filteredJobs = filteredJobs.Where(z => (z.MainJobParticipant != null) == includeSubJobs);
            
            // a magic number here!!!!
            var result = groupingFn(startFilterFunc(endFilterFunc(filteredJobs.Where(x => x.JobTypeID.HasValue)))
                .Where(y => y.JobStatusID != 5))
                .ToList();

            var stats = new StatisticsWithTotals();

            foreach (var group in result)
            {
                stats.Add(group);
            }
            

            return stats;

        }

        public ActionResult DisplayIndebtedCustomers(DateTime? startDate, DateTime? endDate, bool onlyIndebted = false, int? officeID = null, string jobID = null)
        {
            Indebtedness result;

            if (!string.IsNullOrEmpty(jobID))
            {
                result = new Indebtedness(jobID);
            }
            else
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    result = new Indebtedness(startDate.Value, endDate.Value, onlyIndebted, officeID);
                }
                else
                    throw new ArgumentNullException();
            }

            var grouping = result.GetGrouping();
            return PartialView(grouping);
        }

        public class AttractionChannelInfo
        {
            public List<IGrouping<string, Job>> Physicals;
            public List<IGrouping<string, Job>> LegalEntities;
        }

        public ActionResult AttractionChannels(DateTime startDate, DateTime endDate, int officeID = -1)
        {
            startDate = startDate.Date;
            endDate = endDate.Date.AddDays(1);
            var officeName = officeID == -1 ? "Все" : ctx.Offices.Where(x => x.ID == officeID).FirstOrDefault().Name;

            ViewBag.Title = string.Format("Статистика офиса: {0} c {1} по {2}", officeName, startDate.ToString("dd.MM.yyyy"), endDate.ToString("dd.MM.yyyy"));

            var result = ctx.Jobs.Where(x => x.CreatedDate >= startDate 
                && x.CreatedDate < endDate && x.CustomerID == 463); // this counts two subjobs with same # as two; should as one! 
            
            if (officeID > -1)
                result = result.Where(x => x.OfficeID == officeID);
            
            var physicals = result.GroupBy(x => x.AttractionChannel.Name).ToList();

            var newCustomers = ctx.Customers.Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate && x.ID != 463);
            if (officeID > -1)
                newCustomers = newCustomers.Where(x => x.BranchID == officeID);
            var newCustomersIDs = newCustomers.Select(x => x.ID).ToList();
            var legalEntities = ctx.Jobs.Where(x => x.CreatedDate >= startDate && x.CreatedDate < endDate && x.CustomerID != 463 && x.CustomerID.HasValue && newCustomersIDs.Contains(x.CustomerID.Value))
                .GroupBy(x => x.AttractionChannel.Name).ToList();

            var info = new AttractionChannelInfo { Physicals = physicals, LegalEntities = legalEntities };

            ViewData["startDate"] = startDate;
            ViewData["endDate"] = endDate;
            ViewData["officeID"] = officeID;

            return PartialView(info);
        }
        
        [HttpGet]
        public ActionResult SalesLineChart(string startDate, string endDate, string officeID = "", string jobType = "", string officeName = "", string attractionChannel = "")
        {
            var _startDate = DateTime.Parse(startDate).Date;
            var _endDate = DateTime.Parse(endDate).Date.AddDays(1);
            
            var jobs = ctx.Jobs.Where(x => x.CreatedDate >= _startDate && x.CreatedDate <= _endDate);

            // filtering 
            if (officeID != "-1")
            {
                var office = int.Parse(officeID);
                jobs = jobs.Where(x => x.OfficeID == office);
            }
            // 
            if (jobType != "")
            {
                jobs = jobs.Where(x => x.JobType.Name == jobType);
            }

            if (officeName != "")
            {
                jobs = jobs.Where(x => x.Office.Name == officeName);
            }

            if (attractionChannel != "")
            {
                jobs = jobs.Where(x => x.AttractionChannel.Name == attractionChannel);
            }

            var rawJobs = jobs.ToList();

            var tableRawData = rawJobs.GroupBy(x => x.CreatedDate.Value.Date)
                .OrderBy(x=>x.Key)
                .Select(x => new object [] { x.Key.Date, x.Sum(y => y.DiscountedAmount ?? y.AmountFinal ?? 0), x.Sum(y => y.AmountFinal ?? 0) })
                .ToList();

            var allDates = new List<object[]>();
            for (var date = _startDate; date <= _endDate; date = date.AddDays(1))
            {
                allDates.Add(new object[] { date.Date, 0, 0 });
            }

            var filledDates = tableRawData.Select(x => x[0]).ToList();
            allDates = allDates.Where(x => !filledDates.Contains(x[0])).ToList();

            var chartSourceData = tableRawData
                .Union(allDates).OrderBy(x => x[0])
                .Select(x=> new object[] {((DateTime)x[0]).ToString("dd.MM.yy"), x[1], x[2]})
                .ToList();

            chartSourceData.Insert(0, new object[] { "Дата", "Сумма со скидкой", "Сумма без скидки" });


            return Json(chartSourceData, JsonRequestBehavior.AllowGet);
        }
    }
}
